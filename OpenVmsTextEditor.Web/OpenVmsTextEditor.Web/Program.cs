using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenVmsTextEditor.Domain;
using OpenVmsTextEditor.Domain.Interfaces;
using OpenVmsTextEditor.Domain.Literals;
using OpenVmsTextEditor.Domain.Services;
using Serilog;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenVmsTextEditor.Infrastructure;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Set up Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger());

// Configuration
var configuration = builder.Configuration;

// Add services to the container
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// -------------------- Identity (cookies for the site) --------------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI();

builder.Services.AddAuthorization();

// -------------------- Outbound JWT setup (for Java API) -----------------
builder.Services.Configure<JwtIssueOptions>(configuration.GetSection("Jwt")); // Issuer, Audience

// Load RSA private key for signing tokens we send to the Java API
builder.Services.AddSingleton<RsaSecurityKey>(sp =>
{
    // REPLACE with Key Vault or secure storage in production
    var pemPath = configuration["Jwt:PrivateKeyPemPath"] ?? "keys/jwt-private.pem";
    var rsa = RSA.Create();
    rsa.ImportFromPem(File.ReadAllText(pemPath));
    return new RsaSecurityKey(rsa) { KeyId = configuration["Jwt:KeyId"] ?? "kid-1" };
});

// Needed to grab current user in services if you want (optional)
builder.Services.AddHttpContextAccessor();

// Named HttpClient for your Java API
builder.Services.AddHttpClient("JavaApi", http =>
{
    http.BaseAddress = new Uri(configuration["JavaApi:BaseUrl"]!); // e.g. https://java-api.example.com
});
// ------------------------------------------------------------------------

// Razor Pages (Identity UI) + secure folder example
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeFolder("/");                     // secure your pages
        options.Conventions.AllowAnonymousToAreaFolder("Identity", "/"); // but NOT Identity UI
    });

builder.Services.Configure<VmsEditorSettings>(configuration.GetSection(Literal.VmsEditorSettings));

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IOperatingSystemIo, VmsIo>();
builder.Services.AddTransient<IPageInfoService, PageInfoService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UsePathBase("/VmsEditor");
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// -------------------- Example: call Java API with JWT --------------------
// This shows how to mint a short-lived JWT (5 min) and call your Java API.
// In your controllers/services, do the same pattern.
app.MapGet("/reports", async (
    IHttpClientFactory factory,
    ClaimsPrincipal user,
    RsaSecurityKey signingKey,
    IOptions<JwtIssueOptions> jwtOpts) =>
{
    if (!(user.Identity?.IsAuthenticated ?? false))
        return Results.Unauthorized();

    var http = factory.CreateClient("JavaApi");

    // Issue a short-lived "hop" JWT for the Java API
    var token = IssueUserJwt(user, signingKey, jwtOpts.Value);

    http.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    // Example request to Java API
    var resp = await http.GetAsync("/api/data"); // your endpoint
    var contentType = resp.Content.Headers.ContentType?.ToString() ?? "application/json";
    var body = await resp.Content.ReadAsStringAsync();

    return Results.Text(
        body,
        contentType: contentType,
        statusCode: (int)resp.StatusCode
    );
})
.RequireAuthorization(); // ensure only signed-in users can call out
// ------------------------------------------------------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers().RequireAuthorization();

app.MapRazorPages();

app.Run();

// -------------------- Helper: issue JWT for Java API ---------------------
static string IssueUserJwt(ClaimsPrincipal principal, RsaSecurityKey key, JwtIssueOptions opts)
{
    var now = DateTimeOffset.UtcNow;

    var claims = new List<Claim>
    {
        // subject: the user ID from Identity
        new Claim(JwtRegisteredClaimNames.Sub,
            principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty),

        // preferred username / display
        new Claim(ClaimTypes.Name, principal.Identity?.Name ?? string.Empty),

        // include roles if needed
        // .. foreach (var r in principal.FindAll(ClaimTypes.Role)) claims.Add(r);
    };

    var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
    var token = new JwtSecurityToken(
        issuer: opts.Issuer,             // your .NET app
        audience: opts.Audience,         // set to your Java API's expected audience, e.g. "java-api"
        claims: claims,
        notBefore: now.UtcDateTime,
        expires: now.AddMinutes(5).UtcDateTime, // short-lived "hop" token
        signingCredentials: creds);

    token.Header["kid"] = key.KeyId; // support key rotation

    return new JwtSecurityTokenHandler().WriteToken(token);
}

public record JwtIssueOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
}