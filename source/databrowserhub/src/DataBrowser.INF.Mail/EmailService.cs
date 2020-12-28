using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace DataBrowser.INF.Mail
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly MailConfig _mailConfig;
        private readonly GeneralConfig _generalConfig;
        private readonly IRequestContext _requestContext;

        public EmailService(ILogger<EmailService> logger,
                            IOptionsMonitor<MailConfig> mailConfig,
                            IOptionsMonitor<GeneralConfig> generalConfig, 
                            IRequestContext requestContext)
        {
            _logger = logger;
            _mailConfig = mailConfig.CurrentValue;
            _generalConfig = generalConfig.CurrentValue;
            _requestContext = requestContext;
        }

        public async Task RecoveryPasswordAsync(string to, string token)
        {
            _logger.LogDebug("START RecoveryPasswordAsync");

            if (!_mailConfig.Templates.ContainsKey("ResetPassword"))
            {
                _logger.LogDebug("exit RecoveryPasswordAsync template[ResetPassword] config not found");
                return;
            }
            var messagePath = _mailConfig.Templates["ResetPassword"].Message;
            if (string.IsNullOrWhiteSpace(messagePath))
            {
                _logger.LogDebug("exit RecoveryPasswordAsync messagePath config not found");
                return;
            }
            var messagePathLocalized = messagePath.Replace(".html", $".{_requestContext.UserLang}.html".ToLowerInvariant());
            if (!File.Exists(messagePath) &&
               !File.Exists(messagePathLocalized))
            {
                _logger.LogDebug($"exit RecoveryPasswordAsync file not found: {messagePath} ");
                return;
            }
            if (File.Exists(messagePathLocalized))
            {
                _logger.LogDebug($"exit RecoveryPasswordAsync find localized path {messagePathLocalized}");
                messagePath = messagePathLocalized;
            }

            token = HttpUtility.UrlEncode(token);
            var lang = _requestContext.UserLang?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(lang))
            {
                lang = "en";
            }

            /*
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailConfig.DefaultMail);
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = _mailConfig.Templates["ResetPassword"].Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = File.ReadAllText(messagePath).Replace("[TOKEN]", token).Replace("[WEBSITE]", _generalConfig.ExternalClientUrl) };
            */
            MailMessage mail = new MailMessage();
            var SmtpServer = new System.Net.Mail.SmtpClient(_mailConfig.Smtp.Host);

            mail.From = new MailAddress(_mailConfig.DefaultMail);
            mail.To.Add(to);
            mail.Subject = string.IsNullOrWhiteSpace(_mailConfig.Templates["ResetPassword"].Subject) ? "Recovery Password" : _mailConfig.Templates["ResetPassword"].Subject;
            mail.Body = File.ReadAllText(messagePath).Replace("[TOKEN]", token).Replace("[WEBSITE]", _generalConfig.ExternalClientUrl).Replace("[LANG]", lang);
            mail.IsBodyHtml = true;

            SmtpServer.Port = _mailConfig.Smtp.Port;
            SmtpServer.Credentials = new NetworkCredential(_mailConfig.Smtp.Username, _mailConfig.Smtp.Password);
            SmtpServer.EnableSsl = _mailConfig.Smtp.Secure;

            SmtpServer.Send(mail);

            //await senderAsync(email);

            _logger.LogDebug("END RecoveryPasswordAsync");
        }

        public async Task SendAsync(string from, string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(from);
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            await senderAsync(email);
        }

        private async Task senderAsync(MimeMessage email)
        {
            _logger.LogDebug("START senderAsync");

            MailMessage mail = new MailMessage();
            var SmtpServer = new System.Net.Mail.SmtpClient(_mailConfig.Smtp.Host);

            mail.From = new MailAddress(email.Sender.Address);
            mail.To.Add(email.To.Mailboxes.First().Address);
            mail.Subject = string.IsNullOrWhiteSpace(email.Subject) ? "Recovery Password" : email.Subject;
            //mail.Body = email.GetTextBody();

            SmtpServer.Port = _mailConfig.Smtp.Port;
            SmtpServer.Credentials = new NetworkCredential(_mailConfig.Smtp.Username, _mailConfig.Smtp.Password);
            SmtpServer.EnableSsl = _mailConfig.Smtp.Secure;

            SmtpServer.Send(mail);

            //var smtp = new SmtpClient();
            //await smtp.ConnectAsync(_mailConfig.Smtp.Host, _mailConfig.Smtp.Port, _mailConfig.Smtp.Secure);

            //smtp.AuthenticationMechanisms.Clear();
            //smtp.AuthenticationMechanisms.Add("NTLM");
            //smtp.Authenticate(CredentialCache.DefaultNetworkCredentials);
            //smtp.Authenticate(_mailConfig.Smtp.Username, _mailConfig.Smtp.Password);

            //_logger.LogDebug("try to send mail");
            //await smtp.SendAsync(email);
            //_logger.LogDebug("mail sent");
            //smtp.Disconnect(true);

            _logger.LogDebug("END senderAsync");
        }

        private async Task senderAsync2(MimeMessage email)
        {
            _logger.LogDebug("START senderAsync");

            var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_mailConfig.Smtp.Host, _mailConfig.Smtp.Port, _mailConfig.Smtp.Secure);
            
            smtp.AuthenticationMechanisms.Clear();
            smtp.AuthenticationMechanisms.Add("NTLM");
            smtp.Authenticate(CredentialCache.DefaultNetworkCredentials);
            smtp.Authenticate(_mailConfig.Smtp.Username, _mailConfig.Smtp.Password);

            _logger.LogDebug("try to send mail");
            await smtp.SendAsync(email);
            _logger.LogDebug("mail sent");
            smtp.Disconnect(true);

            _logger.LogDebug("END senderAsync");
        }


    }
}
