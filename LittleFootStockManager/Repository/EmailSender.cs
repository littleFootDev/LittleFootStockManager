using LittleFootStockManager.Configuration;
using LittleFootStockManager.Contract;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;


namespace LittleFootStockManager.Repository
{
    public class EmailSender : IEmailSender
    {
        private readonly IOptions<MailOptions> _options;

        public EmailSender(IOptions<MailOptions> Options)
        {
            _options = Options;
        }
        public async Task SendEmail(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_options.Value.Email));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Value.Server, _options.Value.Port, _options.Value.Ssl);
            await client.AuthenticateAsync(_options.Value.Login, _options.Value.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
