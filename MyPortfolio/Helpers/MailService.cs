using Microsoft.Extensions.Options;
using MimeKit;
using MyPortfolio.Helpers.CustomerServiceModels;
using MyPortfolio.Helpers.SettingModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;

namespace MyPortfolio.Helpers
{
    public class MailService(IOptions<MailSettings> _options) : IMailService
    {
        public async Task SendEmailAsync(EmailMessageFormat _emailMessage)
        {
            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(_options.Value.Email),
                Subject = _emailMessage.Subject
            };

            email.To.Add(MailboxAddress.Parse(_emailMessage.To));
            email.From.Add(new MailboxAddress(_options.Value.DisplayName, _options.Value.Email));

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = _emailMessage.Body;

            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_options.Value.Host, _options.Value.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_options.Value.Email, _options.Value.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
