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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenVmsTextEditor.Infrastructure;

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


// Add Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI();


builder.Services.AddRazorPages().AddRazorRuntimeCompilation().AddRazorPagesOptions(options =>
{
    options.Conventions.AuthorizeFolder("/Secure"); // Optional: secure folder
});

builder.Services.Configure<VmsEditorSettings>(configuration.GetSection(Literal.VmsEditorSettings));

var editorSetting = configuration.GetSection(Literal.VmsEditorSettings).Get<VmsEditorSettings>();

builder.Services.AddTransient(opt => Instantiate<IOperatingSystemIo>(
    editorSetting.OperatingSystemIoTypeName,
    opt.GetService<ILoggerFactory>(),
    opt.GetService<IOptions<VmsEditorSettings>>()));

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

app.UseAuthentication(); // Add this
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

// Helper method to instantiate plugin
static T Instantiate<T>(string typeName, ILoggerFactory loggerFactory, IOptions<VmsEditorSettings> settings) where T : class
{
    if (string.IsNullOrWhiteSpace(typeName)) throw new Exception("typeName is missing");

    var pluginType = Type.GetType(typeName);
    if (pluginType == null) throw new Exception($"Could not get Type for {typeName}");
    return (T)Activator.CreateInstance(pluginType, loggerFactory, settings);
}
