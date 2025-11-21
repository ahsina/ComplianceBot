using ComplianceBot.Infrastructure.Identity;
using ComplianceBot.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ComplianceBot.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory for integration tests
/// Configures in-memory database and test services
/// </summary>
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registrations
            RemoveDbContext<ComplianceBotDbContext>(services);
            RemoveDbContext<ApplicationIdentityDbContext>(services);

            // Add in-memory database for testing
            services.AddDbContext<ComplianceBotDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_ComplianceBot");
            });

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_Identity");
            });

            // Build service provider and create database
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var complianceDbContext = scopedServices.GetRequiredService<ComplianceBotDbContext>();
            var identityDbContext = scopedServices.GetRequiredService<ApplicationIdentityDbContext>();

            // Ensure databases are created
            complianceDbContext.Database.EnsureCreated();
            identityDbContext.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }

    private static void RemoveDbContext<T>(IServiceCollection services) where T : DbContext
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<T>));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }
}
