using System.Threading.Tasks;
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

    public async Task<IActionResult> Index(string filter, string startPath)
    {
        _logger.LogDebug("Index(filter={filter},startPath={path})", filter, startPath);
        return View(await _pageInfoService.GetPageInfo(filter, startPath));
    }

    [HttpPost("GetFolder")]
    public async Task<IActionResult> GetFolder(string filter, string path)
    {
        _logger.LogDebug("GetFolder(filter={filter},path={path})", filter, path);
        return Ok(await _pageInfoService.GetPageInfo(filter, path));
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