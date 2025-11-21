using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Common;
using ComplianceBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ComplianceBot.Infrastructure.Persistence;

/// <summary>
/// Main database context for ComplianceBot
/// Implements schema-based multi-tenancy
/// </summary>
public class ComplianceBotDbContext : DbContext, IApplicationDbContext
{
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUserService;

    public ComplianceBotDbContext(
        DbContextOptions<ComplianceBotDbContext> options,
        ITenantContext tenantContext,
        ICurrentUserService currentUserService)
        : base(options)
    {
        _tenantContext = tenantContext;
        _currentUserService = currentUserService;
    }

    // Shared tables (no schema)
    public DbSet<Tenant> Tenants => Set<Tenant>();

    // Tenant-scoped tables (with schema)
    public DbSet<User> Users => Set<User>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ReportDocument> ReportDocuments => Set<ReportDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply schema-based multi-tenancy
        ApplyTenantSchema(modelBuilder);

        // Apply global query filters for tenant isolation
        ApplyGlobalQueryFilters(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit fields before saving
        SetAuditFields();

        // Ensure TenantId is set for all tenant entities
        EnsureTenantId();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTenantSchema(ModelBuilder modelBuilder)
    {
        // Only apply schema if we have a valid tenant context
        if (_tenantContext.TenantId != Guid.Empty)
        {
            var schema = _tenantContext.TenantSchema;

            // Apply schema to all tenant entities
            modelBuilder.Entity<User>().ToTable(nameof(Users), schema);
            modelBuilder.Entity<Report>().ToTable(nameof(Reports), schema);
            modelBuilder.Entity<ReportDocument>().ToTable(nameof(ReportDocuments), schema);
        }
    }

    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // Global query filter for tenant isolation (defense in depth)
        modelBuilder.Entity<User>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<Report>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<ReportDocument>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
    }

    private void SetAuditFields()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.Email;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _currentUserService.Email;
                    break;
            }
        }
    }

    private void EnsureTenantId()
    {
        var entries = ChangeTracker.Entries<TenantEntity>()
            .Where(e => e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            if (entry.Entity.TenantId == Guid.Empty)
            {
                entry.Entity.TenantId = _tenantContext.TenantId;
            }
        }
    }
}
