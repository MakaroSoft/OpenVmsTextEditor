using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace OpenVmsTextEditor.Web.Pages.Error
{
    [AllowAnonymous]
    public class NoPermissionsModel : PageModel
    {
        private readonly ILogger<NoPermissionsModel> _logger;

        public NoPermissionsModel(ILogger<NoPermissionsModel> logger)
        {
            _logger = logger;
        }

        public string? Message { get; private set; }

        public void OnGet(string? message)
        {
            _logger.LogDebug("NoPermissions({message})", message);
            Message = message;
        }
    }
}
