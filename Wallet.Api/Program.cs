using Microsoft.AspNetCore.Identity;
using Serilog;
using Wallet.Api.Extensions;
using Wallet.Data;
using Wallet.Models;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isDevelopment = env == Environments.Development;

IConfiguration config = ConfigurationSetupExtension.GetConfig(isDevelopment);

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

try
{
    logger.Information("Application is starting...");

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContextAndConfigurations(builder.Environment, config);

    // Adds our Authorization Policies to the Dependecy Injection Container
    // services.AddPolicyAuthorization();

    // Configure Identity
    builder.Services.ConfigureIdentity();

    builder.Services.AddAuthentication();
    builder.Services.ConfigureAuthentication(config);

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        try
        {
            Seeder.InitializeDatabase(context, userManager, roleManager).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {

            throw;
        }

    }



    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Application failed to start");
}
finally
{
    logger.Information("Shut down complete");
    Log.CloseAndFlush();
}


