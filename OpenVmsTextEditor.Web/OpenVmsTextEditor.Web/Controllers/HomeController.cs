using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVmsTextEditor.Domain.Interfaces;
using OpenVmsTextEditor.Domain.Services;

namespace OpenVmsTextEditor.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IOperatingSystemIo _operatingSystemIo;
    private readonly IPageInfoService _pageInfoService;
    private readonly SignInManager<IdentityUser> _signInManager;

    public HomeController(ILogger<HomeController> logger,
        IOperatingSystemIo operatingSystemIo, 
        IPageInfoService pageInfoService,
        SignInManager<IdentityUser> signInManager) 
    {
        _logger = logger;
        _operatingSystemIo = operatingSystemIo;
        _pageInfoService = pageInfoService;
        _signInManager = signInManager;
    }

    [HttpGet]
    [AllowAnonymous] // optional, but fine
    public async Task<IActionResult> Logout(string? returnUrl)
    {
        await _signInManager.SignOutAsync();

        // Redirect back to the page the user came from (with query string)
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        // Fallback: app root under your PathBase (/VmsEditor)
        return Redirect($"{Request.PathBase}/");
    }
    
    public async Task<IActionResult> Index(string? include, string? exclude, bool showHistory, string startPath, CancellationToken ct)
    {
        _logger.LogDebug("Index(include={include}, exclude={exclude}, showHistory={showHistory}, startPath={path})", include, exclude, showHistory, startPath);
        ViewBag.ShowHistory = showHistory? "Checked" : "";
        ViewBag.Include = include ?? "";
        ViewBag.Exclude = exclude ?? "";

        try
        {
            var model = await _pageInfoService.GetPageInfoAsync(include, exclude, showHistory, startPath, ct);
            return View(model);
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.Forbidden)
        {
            // Logout
            await _signInManager.SignOutAsync();

            // Redirect to custom error page with message
            return RedirectToAction("NoPermissions", "Error");
        }
        catch (Exception e)
        {
            // Logout
            await _signInManager.SignOutAsync();
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    [HttpPost("GetFolder")]
    public async Task<IActionResult> GetFolder(string? include, string? exclude, bool showHistory, string? path, CancellationToken ct)
    {
        _logger.LogDebug("GetFolder(include={include}, exclude={exclude}, showHistory={showHistory}, startPath={path})", include, exclude, showHistory, path);
        return Ok(await _pageInfoService.GetPageInfoAsync(include, exclude, showHistory, path, ct));
    }

    [HttpPost("GetFile")]
    public async Task<IActionResult> GetFile(string path, CancellationToken ct)
    {
        _logger.LogDebug("GetFile(path={path})", path);
        return Ok(await _operatingSystemIo.GetFileAsync(path, ct));
    }

    [HttpPost("SaveFile")]
    public async Task<IActionResult> SaveFile(string path, [FromBody] string fileData, CancellationToken ct)
    {
        _logger.LogDebug("SaveFile(path={path})", path);
        return Ok(await _operatingSystemIo.SaveFileAsync(path, fileData, ct));
    }

}