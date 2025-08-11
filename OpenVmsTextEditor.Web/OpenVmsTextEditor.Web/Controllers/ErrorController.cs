using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OpenVmsTextEditor.Web.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public ErrorController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult Index(string? message)
    {
        _logger.LogDebug("Index()");
        ViewBag.Message = message ?? "An unexpected error occurred.";
        return View();
    }
    
    [AllowAnonymous]
    public IActionResult NoPermissions(string? message)
    {
        _logger.LogDebug("NoPermissions({message})", message);
        ViewBag.Message = message;
        return View();
    }
}