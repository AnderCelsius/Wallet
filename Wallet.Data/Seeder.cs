using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Models;

namespace Wallet.Data
{
    public class Seeder
    {
        public static async Task SeedData(WalletDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var baseDir = Directory.GetCurrentDirectory();

            await dbContext.Database.EnsureCreatedAsync();

            if (!dbContext.Users.Any())
            {
                List<string> roles = new List<string> { "Admin", "Customer" };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = role });
                }

                var user = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Obinna Asiegbulam",
                    UserName = "oasiegbulam@gmail.com",
                    Email = "oasiegbulam@gmail.com",
                    PhoneNumber = "09043546576",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                user.EmailConfirmed = true;
                await userManager.CreateAsync(user, "Password@123");
                await userManager.AddToRoleAsync(user, "Admin");

                var path = File.ReadAllText(FilePath(baseDir, "Json/users.json"));

                var users = JsonConvert.DeserializeObject<List<AppUser>>(path);
                for (int i = 0; i < users.Count; i++)
                {
                    users[i].EmailConfirmed = true;
                    await userManager.CreateAsync(users[i], "Password@123");
                    await userManager.AddToRoleAsync(users[i], "Customer");
                }
            }


            // Bookings and Payment
            if (!dbContext.Products.Any())
            {
                var path = File.ReadAllText(FilePath(baseDir, "Json/products.json"));

                var products = JsonConvert.DeserializeObject<List<Product>>(path);
                await dbContext.Products.AddRangeAsync(products);
            }

            // Whishlist
            if (!dbContext.Wallets.Any())
            {
                var path = File.ReadAllText(FilePath(baseDir, "Json/wallets.json"));

                var wallets = JsonConvert.DeserializeObject<List<UserWallet>>(path);
                await dbContext.Wallets.AddRangeAsync(wallets);
            }


            await dbContext.SaveChangesAsync();
        }

        static string FilePath(string folderName, string fileName)
        {
            return Path.Combine(folderName, fileName);
        }
    }
}
