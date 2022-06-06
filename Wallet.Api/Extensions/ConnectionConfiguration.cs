using Microsoft.EntityFrameworkCore;
using Wallet.Data;

namespace Wallet.Api.Extensions
{
    public static class ConnectionConfiguration
    {
        public static void AddDbContextAndConfigurations(this IServiceCollection services, IWebHostEnvironment env, IConfiguration config)
        {
            services.AddDbContextPool<WalletDbContext>(options =>
            {
                string connStr = string.Empty;

                if (!env.IsProduction())
                {
                    connStr = config.GetConnectionString("Default");
                }
                options.UseNpgsql(connStr);
            });
        }
    }
}
