namespace Wallet.Models
{
    public class UserWallet : BaseEntity
    {
        public double Balance { get; set; }

        //Foreign key for User
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
