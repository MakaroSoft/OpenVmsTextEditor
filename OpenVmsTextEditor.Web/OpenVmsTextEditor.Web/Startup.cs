using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenVmsTextEditor.Domain;
using OpenVmsTextEditor.Domain.Interfaces;
using OpenVmsTextEditor.Domain.Literals;
using OpenVmsTextEditor.Domain.Services;

namespace OpenVmsTextEditor.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            // Use the default property (Pascal) casing.
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
            
        services.AddRazorPages().AddRazorRuntimeCompilation();
        services.Configure<VmsEditorSettings>(Configuration.GetSection(Literal.VmsEditorSettings));

        var editorSetting = Configuration.GetSection(Literal.VmsEditorSettings).Get<VmsEditorSettings>();

        services.AddTransient(opt => Instantiate<IOperatingSystemIo>(editorSetting.OperatingSystemIoTypeName, 
            opt.GetService<ILoggerFactory>(),
            opt.GetService<IOptions<VmsEditorSettings>>()));

        services.AddTransient<IPageInfoService, PageInfoService>();

    }

    private T Instantiate<T>(string typeName, ILoggerFactory loggerFactory, IOptions<VmsEditorSettings> settings) where T : class
    {
        if (string.IsNullOrWhiteSpace(typeName)) throw new Exception("typeName is missing");

        var pluginType = Type.GetType(typeName);
        if (pluginType == null) throw new Exception($"Could not get Type for {typeName}");
        return (T)Activator.CreateInstance(pluginType, loggerFactory, settings);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            //app.UseExceptionHandler("/Error");
            app.UseDeveloperExceptionPage();
            app.UsePathBase("/VmsEditor");
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}