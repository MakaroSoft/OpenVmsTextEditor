using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OpenVmsTextEditor.Domain.Interfaces;
using OpenVmsTextEditor.Domain.Services;
using OpenVmsTextEditor.Domain.Models;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenVmsTextEditor.Web.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IOperatingSystemIo _operatingSystemIo;
        private readonly IPageInfoService _pageInfoService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(ILogger<IndexModel> logger,
            IOperatingSystemIo operatingSystemIo,
            IPageInfoService pageInfoService,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _operatingSystemIo = operatingSystemIo;
            _pageInfoService = pageInfoService;
            _signInManager = signInManager;
        }

        [BindProperty(SupportsGet = true)]
        public string? Include { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Exclude { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ShowHistory { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StartPath { get; set; }

        public VmsEditorModel VmsEditor { get; set; } = new VmsEditorModel();

        public async Task<IActionResult> OnGetAsync(CancellationToken ct)
        {
            _logger.LogDebug("Index(include={include}, exclude={exclude}, showHistory={showHistory}, startPath={path})", Include, Exclude, ShowHistory, StartPath);

            try
            {
                var model = await _pageInfoService.GetPageInfoAsync(Include, Exclude, ShowHistory, StartPath, ct);
                VmsEditor = model;
                return Page();
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.Forbidden)
            {
                // Logout
                await _signInManager.SignOutAsync();

                // Redirect to custom error page with message
                return RedirectToPage("/Error/NoPermissions");
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
        public async Task<IActionResult> OnPostGetFolderAsync(string? include, string? exclude, bool showHistory, string? path, CancellationToken ct)
        {
            _logger.LogDebug("GetFolder(include={include}, exclude={exclude}, showHistory={showHistory}, startPath={path})", include, exclude, showHistory, path);
            return new JsonResult(await _pageInfoService.GetPageInfoAsync(include, exclude, showHistory, path, ct));
        }

        [HttpPost("GetFile")]
        public async Task<IActionResult> OnPostGetFileAsync(string path, CancellationToken ct)
        {
            _logger.LogDebug("GetFile(path={path})", path);
            return new JsonResult(await _operatingSystemIo.GetFileAsync(path, ct));
        }

        [HttpPost("SaveFile")]
        public async Task<IActionResult> OnPostSaveFileAsync(string path, [FromBody] string fileData, CancellationToken ct)
        {
            _logger.LogDebug("SaveFile(path={path})", path);
            return new JsonResult(await _operatingSystemIo.SaveFileAsync(path, fileData, ct));
        }
    }
}
