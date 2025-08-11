using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace OpenVmsTextEditor.Web.Pages.Error
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public string Message { get; private set; } = string.Empty;

        public void OnGet(string? message)
        {
            _logger.LogDebug("Error Index()");
            Message = string.IsNullOrWhiteSpace(message) ? "An unexpected error occurred." : message;
        }
    }
}
