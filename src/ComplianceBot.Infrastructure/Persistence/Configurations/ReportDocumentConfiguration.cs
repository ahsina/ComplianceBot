using ComplianceBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceBot.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for ReportDocument entity
/// </summary>
public class ReportDocumentConfiguration : IEntityTypeConfiguration<ReportDocument>
{
    public void Configure(EntityTypeBuilder<ReportDocument> builder)
    {
        // Table will be configured with schema in DbContext

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DocumentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.BlobStoragePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(d => d.FileHash)
            .HasMaxLength(256);

        // Indexes for performance
        builder.HasIndex(d => new { d.TenantId, d.ReportId })
            .HasDatabaseName("IX_ReportDocuments_TenantId_ReportId");

        builder.HasIndex(d => d.DocumentType)
            .HasDatabaseName("IX_ReportDocuments_DocumentType");

        // Relationship to report
        builder.HasOne(d => d.Report)
            .WithMany(r => r.Documents)
            .HasForeignKey(d => d.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
