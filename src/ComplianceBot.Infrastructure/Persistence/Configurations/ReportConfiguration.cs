using ComplianceBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceBot.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Report entity
/// </summary>
public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        // Table will be configured with schema in DbContext

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.ReportingPeriod)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.ValidationResult)
            .HasMaxLength(4000);

        builder.Property(r => r.Notes)
            .HasMaxLength(2000);

        // Indexes for performance
        builder.HasIndex(r => new { r.TenantId, r.Type, r.ReportingPeriod });
        builder.HasIndex(r => new { r.TenantId, r.Status });
        builder.HasIndex(r => r.DueDate);

        // Relationships
        builder.HasOne(r => r.SubmittedByUser)
            .WithMany()
            .HasForeignKey(r => r.SubmittedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(r => r.Documents)
            .WithOne(d => d.Report)
            .HasForeignKey(d => d.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
