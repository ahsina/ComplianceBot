using ComplianceBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// Database context interface for the Application layer
/// Follows Dependency Inversion Principle
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<User> Users { get; }
    DbSet<Report> Reports { get; }
    DbSet<ReportDocument> ReportDocuments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
