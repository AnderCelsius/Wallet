using Wallet.Models.Mail;

namespace Wallet.Core.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(ICollection<MailRequest> mailRequest);
    }
}
