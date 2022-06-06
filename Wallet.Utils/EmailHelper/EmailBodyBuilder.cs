using System.Globalization;
using Wallet.Models;

namespace Wallet.Utils.EmailHelper
{
    public class EmailBodyBuilder
    {
        public static string GetEmailBody(AppUser user, string linkName, string token, string controllerName)
        {
            var link = $"https://localhost:7198/{controllerName}/{linkName}?email={user.Email}&token={token}";
            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
            var userName = textInfo.ToTitleCase(user.Name);
            var emailBody = $"Hello {userName}\n\nWelcome to Wallet.io. Click on the link below to complete your registration.\n\n{link}";
            return emailBody;
        }
    }
}
