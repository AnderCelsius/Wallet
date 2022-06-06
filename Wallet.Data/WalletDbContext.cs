using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wallet.Models;

namespace Wallet.Data
{
    public class WalletDbContext : IdentityDbContext<AppUser>
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserWallet> Wallets { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries<BaseEntity>())
            {
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Added:
                        item.Entity.Id = Guid.NewGuid().ToString();
                        item.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    default:
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        //private void SeedUsers(ModelBuilder builder)
        //{
        //    List<AppUser> user = new()
        //    {
        //        new AppUser{Id = Guid.NewGuid().ToString(), Name = "Obinna Asiegbulam", Email = "oasiegbulam@gmail.com"},
        //        new AppUser{Id = Guid.NewGuid().ToString(), Name = "Osbourn Schoular", Email = "oschoular0@ftc.gov"},
        //        new AppUser{Id = Guid.NewGuid().ToString(), Name = "Arni Antecki", Email = "aantecki1@squidoo.com"},
        //        new AppUser{Id = Guid.NewGuid().ToString(), Name = "Murvyn Malek", Email = "mmalek2@shop-pro.jp"},
        //        new AppUser{Id = Guid.NewGuid().ToString(), Name = "Gusta Kaysor", Email = "gkaysor3@ucoz.ru"},
        //    };
        //}
    }
}