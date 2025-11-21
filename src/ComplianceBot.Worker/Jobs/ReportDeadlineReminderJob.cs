using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComplianceBot.Worker.Jobs;

/// <summary>
/// Background job to send reminders for upcoming report deadlines
/// Runs daily to check for reports due in the next 7 days
/// </summary>
public class ReportDeadlineReminderJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ReportDeadlineReminderJob> _logger;

    public ReportDeadlineReminderJob(
        IApplicationDbContext context,
        ILogger<ReportDeadlineReminderJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting report deadline reminder job");

        var today = DateTime.UtcNow.Date;
        var reminderDate = today.AddDays(7);

        // Find reports due in the next 7 days that are still in Draft or Validated status
        var upcomingReports = await _context.Reports
            .Where(r => r.DueDate.HasValue &&
                        r.DueDate.Value.Date <= reminderDate &&
                        r.DueDate.Value.Date >= today &&
                        (r.Status == ReportStatus.Draft || r.Status == ReportStatus.Validated))
            .Include(r => r.Tenant)
            .ToListAsync();

        _logger.LogInformation(
            "Found {Count} reports with upcoming deadlines",
            upcomingReports.Count);

        foreach (var report in upcomingReports)
        {
            var daysUntilDue = (report.DueDate.Value.Date - today).Days;

            _logger.LogInformation(
                "Report {ReportId} ({Type}) for tenant {TenantName} is due in {Days} days",
                report.Id,
                report.Type,
                report.Tenant?.Name,
                daysUntilDue);

            // TODO: Send email notification
            // await _emailService.SendDeadlineReminderAsync(report, daysUntilDue);
        }

        _logger.LogInformation("Completed report deadline reminder job");
    }
}
