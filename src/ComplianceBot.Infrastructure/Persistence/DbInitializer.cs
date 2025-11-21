using ComplianceBot.Domain.Entities;
using ComplianceBot.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComplianceBot.Infrastructure.Persistence;

/// <summary>
/// Database initializer for seeding sample data
/// </summary>
public class DbInitializer
{
    private readonly ComplianceBotDbContext _context;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(ComplianceBotDbContext context, ILogger<DbInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with sample data for development/testing
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (await _context.Tenants.AnyAsync())
            {
                _logger.LogInformation("Database already seeded");
                return;
            }

            _logger.LogInformation("Seeding database...");

            // Seed tenants
            var tenants = GetSampleTenants();
            _context.Tenants.AddRange(tenants);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} tenants", tenants.Count);

            // TODO: Seed users, reports, etc.
            // Note: Actual schema-based seeding would require schema creation first

            _logger.LogInformation("Database seeding completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            throw;
        }
    }

    private List<Tenant> GetSampleTenants()
    {
        return new List<Tenant>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Acme Luxembourg S.A.",
                CompanyRegistrationNumber = "LU12345678",
                VATNumber = "LU12345678",
                Country = "LU",
                City = "Luxembourg",
                Address = "1 Rue de la Banque",
                PostalCode = "L-1234",
                PrimaryContactName = "Jean Dupont",
                PrimaryContactEmail = "jean.dupont@acme.lu",
                PrimaryContactPhone = "+352 123 456",
                SubscriptionTier = SubscriptionTier.Professional,
                SubscriptionStartDate = DateTime.UtcNow,
                IsActive = true,
                EnabledReportTypes = new List<ReportType>
                {
                    ReportType.RBE,
                    ReportType.CEDR,
                    ReportType.FATCA
                },
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Global Investments SICAV",
                CompanyRegistrationNumber = "LU23456789",
                VATNumber = "LU23456789",
                Country = "LU",
                City = "Luxembourg",
                Address = "50 Avenue J.F. Kennedy",
                PostalCode = "L-2951",
                PrimaryContactName = "Marie Laurent",
                PrimaryContactEmail = "marie.laurent@globalinv.lu",
                PrimaryContactPhone = "+352 234 567",
                SubscriptionTier = SubscriptionTier.Enterprise,
                SubscriptionStartDate = DateTime.UtcNow,
                IsActive = true,
                EnabledReportTypes = new List<ReportType>
                {
                    ReportType.RBE,
                    ReportType.CEDR,
                    ReportType.FATCA,
                    ReportType.CRS,
                    ReportType.SFDR
                },
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };
    }
}
