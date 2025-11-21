using ComplianceBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceBot.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Tenant entity
/// </summary>
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants"); // Shared table, no schema

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.CompanyRegistrationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.VATNumber)
            .HasMaxLength(50);

        builder.Property(t => t.Country)
            .IsRequired()
            .HasMaxLength(2); // ISO 3166-1 alpha-2

        builder.Property(t => t.PrimaryContactEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(t => t.CompanyRegistrationNumber)
            .IsUnique();

        // Owned collection for enabled report types (stored as JSON)
        builder.Property(t => t.EnabledReportTypes)
            .HasConversion(
                v => string.Join(',', v.Select(x => (int)x)),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => (ComplianceBot.Domain.Enums.ReportType)int.Parse(x))
                    .ToList()
            );
    }
}
