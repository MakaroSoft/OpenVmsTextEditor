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
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Error/NoPermissions");
            }
            catch (Exception e)
            {
                await _signInManager.SignOutAsync();
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
