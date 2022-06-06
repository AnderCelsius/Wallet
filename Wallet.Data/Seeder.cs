using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
        public static async Task InitializeDatabase(WalletDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var baseDir = Directory.GetCurrentDirectory();

            await dbContext.Database.EnsureCreatedAsync();

            if (!dbContext.Users.Any() && userManager != null && roleManager != null)
            {
                List<string> roles = new() { "Admin", "Customer" };

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
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                user.EmailConfirmed = true;
                var wallet = new UserWallet()
                {
                    Balance = 500
                };
                user.Wallet = wallet;
                
                await userManager.CreateAsync(user, "Password@123");
                await userManager.AddToRoleAsync(user, "Admin");

                var path = File.ReadAllText(FilePath(baseDir, "Json/users.json"));

                var users = JsonConvert.DeserializeObject<List<AppUser>>(path);
                for (int i = 0; i < users.Count; i++)
                {
                    users[i].CreatedAt = DateTime.UtcNow;
                    users[i].UpdatedAt = DateTime.UtcNow;
                    users[i].EmailConfirmed = true;
                    users[i].UserName = users[i].Email;
                    await userManager.CreateAsync(users[i], "Password@123");
                    await userManager.AddToRoleAsync(users[i], "Customer");
                }
            }


            // Products
            if (!dbContext.Products.Any())
            {
                var path = File.ReadAllText(FilePath(baseDir, "Json/products.json"));

                var products = JsonConvert.DeserializeObject<List<Product>>(path);
                await dbContext.Products.AddRangeAsync(products);
            }


            await dbContext.SaveChangesAsync();
        }

        static string FilePath(string folderName, string fileName)
        {
            return Path.Combine(folderName, fileName);
        }
    }
}
