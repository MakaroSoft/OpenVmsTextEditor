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

    public IActionResult Index()
    {
        _logger.LogDebug("Index()");
        return View();
    }

}