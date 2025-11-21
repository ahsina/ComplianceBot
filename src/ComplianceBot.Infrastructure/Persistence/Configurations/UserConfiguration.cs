using ComplianceBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceBot.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table will be configured with schema in DbContext

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(u => u.IdentityId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Role)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique()
            .HasDatabaseName("IX_Users_TenantId_Email");

        builder.HasIndex(u => u.IdentityId)
            .IsUnique()
            .HasDatabaseName("IX_Users_IdentityId");

        builder.HasIndex(u => new { u.TenantId, u.IsActive })
            .HasDatabaseName("IX_Users_TenantId_IsActive");

        // Relationship to tenant
        builder.HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
