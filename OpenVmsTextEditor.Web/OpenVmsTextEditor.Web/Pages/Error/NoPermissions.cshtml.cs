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

        public void OnGet()
        {
            _logger.LogDebug("No Permission");
        }
    }
}
