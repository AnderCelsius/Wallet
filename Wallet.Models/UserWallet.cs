namespace Wallet.Models
{
    public class UserWallet : BaseEntity
    {
        public UserWallet(AppUser user)
        {
            User = user;
            UserId = user.Id;
        }
       
        public double Balance { get; set; }

        //Foreign key for User
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
