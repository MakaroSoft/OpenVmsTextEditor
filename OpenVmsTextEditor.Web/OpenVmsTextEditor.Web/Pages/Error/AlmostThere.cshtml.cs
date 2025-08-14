using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace OpenVmsTextEditor.Web.Pages.Error
{
    [AllowAnonymous]
    public class AlmostThereModel : PageModel
    {
        private readonly ILogger<AlmostThereModel> _logger;

        public AlmostThereModel(ILogger<AlmostThereModel> logger)
        {
            _logger = logger;
        }

        public string? Message { get; private set; }

        public void OnGet(string? message)
        {
            _logger.LogDebug("AlmostThere({message})", message);
            Message = message;
        }
    }
}


