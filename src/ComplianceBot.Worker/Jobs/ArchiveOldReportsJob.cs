using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComplianceBot.Worker.Jobs;

/// <summary>
/// Background job to archive old submitted reports
/// Archives reports that were submitted more than 2 years ago
/// </summary>
public class ArchiveOldReportsJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ArchiveOldReportsJob> _logger;

    public ArchiveOldReportsJob(
        IApplicationDbContext context,
        ILogger<ArchiveOldReportsJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting archive old reports job");

        var archiveDate = DateTime.UtcNow.AddYears(-2);

        // Find submitted reports older than 2 years
        var reportsToArchive = await _context.Reports
            .Where(r => r.Status == ReportStatus.Submitted &&
                        r.SubmittedAt.HasValue &&
                        r.SubmittedAt.Value < archiveDate)
            .ToListAsync();

        _logger.LogInformation(
            "Found {Count} reports to archive",
            reportsToArchive.Count);

        foreach (var report in reportsToArchive)
        {
            report.Status = ReportStatus.Archived;
            report.ModifiedAt = DateTime.UtcNow;
            report.ModifiedBy = "System (Archive Job)";

            _logger.LogInformation(
                "Archived report {ReportId} ({Type}) submitted on {SubmittedAt}",
                report.Id,
                report.Type,
                report.SubmittedAt);
        }

        if (reportsToArchive.Any())
        {
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Completed archive old reports job");
    }
}
