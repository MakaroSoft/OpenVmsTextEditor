using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IPageInfoService _pageInfoService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(ILogger<IndexModel> logger,
            IOperatingSystemIo operatingSystemIo,
            IPageInfoService pageInfoService,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
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

        public VmsEditorModel VmsEditor { get; set; } = new VmsEditorModel
        {
            Disks = new List<string>(),
            Files = new List<File>(),
            BreadCrumb = new List<string>()
        };

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
                // If user has no roles yet, show Almost There; else show No Permission
                var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role);
                bool hasAnyRole = roles.Any();

                if (hasAnyRole)
                {
                    return RedirectToPage("/Error/NoPermissions");
                }
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Error/AlmostThere");
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
