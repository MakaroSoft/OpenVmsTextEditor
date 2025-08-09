using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenVmsTextEditor.Domain;
using OpenVmsTextEditor.Domain.Interfaces;
using OpenVmsTextEditor.Domain.Literals;
using OpenVmsTextEditor.Domain.Services;
using Serilog;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenVmsTextEditor.Infrastructure;

using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

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
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI();

builder.Services.AddAuthorization();

// -------------------- Outbound JWT setup (for Java API) -----------------
//builder.Services.Configure<JwtIssueOptions>(configuration.GetSection("Jwt")); // Issuer, Audience
builder.Services
    .AddOptions<JwtIssueOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "Jwt.Issuer is required")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "Jwt.Audience is required")
    .ValidateOnStart();



// Load RSA private key for signing tokens we send to the Java API
builder.Services.AddSingleton<RsaSecurityKey>(_ =>
{
    // REPLACE with Key Vault or secure storage in production
    var pemPath = configuration["Jwt:PrivateKeyPemPath"] ?? "keys/jwt-private.pem";
    var rsa = RSA.Create();
    rsa.ImportFromPem(File.ReadAllText(pemPath));
    return new RsaSecurityKey(rsa) { KeyId = configuration["Jwt:KeyId"] ?? "kid-1" };
});

builder.Services.AddTransient<JwtHopHandler>();

// Needed to grab current user in services if you want (optional)
builder.Services.AddHttpContextAccessor();

// Razor Pages (Identity UI) + secure folder example
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeFolder("/");                     // secure your pages
        options.Conventions.AllowAnonymousToAreaFolder("Identity", "/"); // but NOT Identity UI
    });

builder.Services.Configure<VmsEditorSettings>(configuration.GetSection(Literal.VmsEditorSettings));

builder.Services.AddHttpClient<IOpenVmsExplorerApiClient, OpenVmsExplorerApiClient>(http =>
{
    http.BaseAddress = new Uri(builder.Configuration["VmsEditorSettings:VmsExplorerApiUrl"] ?? string.Empty);
    http.DefaultRequestHeaders.Accept.ParseAdd("application/json");
}).AddHttpMessageHandler<JwtHopHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IOperatingSystemIo, VmsIo>();
builder.Services.AddTransient<IPageInfoService, PageInfoService>();

var app = builder.Build();

// Program.cs after app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();                 // applies pending migrations

    await SeedAsync(scope.ServiceProvider); // optional custom seed (see below)
}


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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers().RequireAuthorization();

app.MapRazorPages();

app.Run();

static async Task SeedAsync(IServiceProvider services)
{
    // This is outside of the project so I don't accidentally push it to github :)
    var configPath = @"C:\repos\vmsEditorConfig.json";
    if (!File.Exists(configPath))
    {
        throw new FileNotFoundException($"Config file not found: {configPath}");
    }

    var json = await File.ReadAllTextAsync(configPath);
    var config = JsonSerializer.Deserialize<SeedConfig>(json) 
                 ?? throw new InvalidOperationException("Invalid config file format.");

    var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = services.GetRequiredService<UserManager<IdentityUser>>();

    // Create roles if they don't exist
    string[] roles = { "Admin", "SuperUser", "User" };
    foreach (var r in roles)
    {
        if (!await roleMgr.RoleExistsAsync(r))
        {
            await roleMgr.CreateAsync(new IdentityRole(r));
        }
    }

    // Create admin if it doesn't exist
    var admin = await userMgr.FindByEmailAsync(config.AdminEmail);
    if (admin == null)
    {
        admin = new IdentityUser
        {
            UserName = config.AdminEmail,
            Email = config.AdminEmail,
            EmailConfirmed = true
        };

        var createResult = await userMgr.CreateAsync(admin, config.AdminPassword);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
        }

        await userMgr.AddToRoleAsync(admin, "Admin");
    }
}

// Strongly typed config model
public class SeedConfig
{
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
}
