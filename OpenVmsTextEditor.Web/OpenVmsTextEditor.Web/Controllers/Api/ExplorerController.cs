using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVmsTextEditor.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using OpenVmsTextEditor.Domain.Services;

namespace OpenVmsTextEditor.Web.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExplorerController : ControllerBase
    {
        private readonly ILogger<ExplorerController> _logger;
        private readonly IOperatingSystemIo _operatingSystemIo;
        private readonly IPageInfoService _pageInfoService;

        public ExplorerController(
            ILogger<ExplorerController> logger,
            IOperatingSystemIo operatingSystemIo,
            IPageInfoService pageInfoService)
        {
            _logger = logger;
            _operatingSystemIo = operatingSystemIo;
            _pageInfoService = pageInfoService;
        }

        [HttpPost("folder")]
        public async Task<IActionResult> Folder(string? include, string? exclude, bool showHistory, string? path, CancellationToken ct)
        {
            _logger.LogDebug("Folder(include={include}, exclude={exclude}, showHistory={showHistory}, path={path})", include, exclude, showHistory, path);
            return Ok(await _pageInfoService.GetPageInfoAsync(include, exclude, showHistory, path, ct));
        }

        [HttpPost("file")]
        public async Task<IActionResult> File(string path, CancellationToken ct)
        {
            _logger.LogDebug("File(path={path})", path);
            return Ok(await _operatingSystemIo.GetFileAsync(path, ct));
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save(string path, [FromBody] string fileData, CancellationToken ct)
        {
            _logger.LogDebug("Save(path={path})", path);
            return Ok(await _operatingSystemIo.SaveFileAsync(path, fileData, ct));
        }
    }
}
