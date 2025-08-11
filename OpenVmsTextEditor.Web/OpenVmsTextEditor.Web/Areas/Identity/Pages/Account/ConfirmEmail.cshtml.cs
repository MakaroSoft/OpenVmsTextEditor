// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Encodings.Web;

namespace OpenVmsTextEditor.Web.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        
        public string? ReturnUrl { get; private set; }
        
        public async Task<IActionResult> OnGetAsync(string userId, string code, string? returnUrl = null)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            // Use the SAME returnUrl we carried from Register (fallback to app root)
            ReturnUrl = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : Url.Content("~");
            
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

            if (result.Succeeded)
            {
                var adminEmail = _configuration["VmsEditorSettings:AdminNotificationsEmail"];
                if (!string.IsNullOrWhiteSpace(adminEmail))
                {
                    var subject = "User activated account from the OpenVmsTextEditor";
                    var loginUrl = Url.Page("/Account/Login", pageHandler: null, values: new { area = "Identity" }, protocol: Request.Scheme);
                    var body = $"User {user.Email} confirmed their email at {DateTime.UtcNow:u}.<br/>" +
                               $"Login to the web app: <a href='{HtmlEncoder.Default.Encode(loginUrl)}'>Open login</a>";
                    await _emailSender.SendEmailAsync(adminEmail, subject, body);
                }
            }

            return Page();
        }
    }
}
