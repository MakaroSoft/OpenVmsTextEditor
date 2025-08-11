using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace OpenVmsTextEditor.Infrastructure;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _opt;
    public SmtpEmailSender(IOptions<SmtpOptions> opt) => _opt = opt.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using var client = new System.Net.Mail.SmtpClient(_opt.Host, _opt.Port)
        {
            EnableSsl = _opt.EnableSsl,
            Credentials = new System.Net.NetworkCredential(_opt.User, _opt.Password)
        };
        using var msg = new System.Net.Mail.MailMessage(_opt.From, email, subject, htmlMessage)
            { IsBodyHtml = true };
        await client.SendMailAsync(msg);
    }
}
