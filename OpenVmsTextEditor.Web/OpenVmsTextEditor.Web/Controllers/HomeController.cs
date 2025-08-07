using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

    public HomeController(ILogger<HomeController> logger,
        IOperatingSystemIo operatingSystemIo, IPageInfoService pageInfoService) 
    {
        _logger = logger;
        _operatingSystemIo = operatingSystemIo;
        _pageInfoService = pageInfoService;
    }

    [Authorize]
    public async Task<IActionResult> Index(string? include, string? exclude, bool showHistory, string startPath)
    {
        _logger.LogDebug("Index(include={include}, exclude={exclude}, showHistory={showHistory}, startPath={path})", include, exclude, showHistory, startPath);
        ViewBag.ShowHistory = showHistory == true? "Checked" : "";
        ViewBag.Include = include ?? "";
        ViewBag.Exclude = exclude ?? "";
        return View(await _pageInfoService.GetPageInfo(include, exclude, showHistory, startPath));
    }

    [HttpPost("GetFolder")]
    public async Task<IActionResult> GetFolder(string? include, string? exclude, bool showHistory, string path)
    {
        _logger.LogDebug("GetFolder(include={include}, exclude={exclude}, showHistory={showHistory}, startPath={path})", include, exclude, showHistory, path);
        return Ok(await _pageInfoService.GetPageInfo(include, exclude, showHistory, path));
    }

    [HttpPost("GetFile")]
    public async Task<IActionResult> GetFile(string path)
    {
        _logger.LogDebug("GetFile(path={path})", path);
        return Ok(await _operatingSystemIo.GetFile(path));
    }

    [HttpPost("SaveFile")]
    public async Task<IActionResult> SaveFile(string path, [FromBody] string fileData)
    {
        _logger.LogDebug("SaveFile(path={path})", path);
        return Ok(await _operatingSystemIo.SaveFile(path, fileData));
    }

}