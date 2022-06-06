using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Wallet.Core.Interfaces;
using Wallet.Models.Mail;

namespace Wallet.Core.Implementations
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(MailSettings mailSettings)
        {
            _mailSettings = mailSettings;
        }
        public async Task<bool> SendEmailAsync(ICollection<MailRequest> mailRequest)
        {
            var email = new MimeMessage { Sender = MailboxAddress.Parse(_mailSettings.Mail) };
            foreach (var item in mailRequest)
            {
                email.To.Add(MailboxAddress.Parse(item.ToEmail));
                email.Subject = item.Subject;

                var builder = new BodyBuilder();
                if (item.Attachments != null)
                {
                    foreach (var file in item.Attachments.Where(file => file.Length > 0))
                    {
                        byte[] fileBytes;
                        await using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add((file.FileName + Guid.NewGuid().ToString()), fileBytes, ContentType.Parse(file.ContentType));
                    }
                }

                builder.HtmlBody = item.Body;
                email.Body = builder.ToMessageBody();
            }
            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
