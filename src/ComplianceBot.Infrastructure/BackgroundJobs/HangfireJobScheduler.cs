using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Worker.Jobs;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace ComplianceBot.Infrastructure.BackgroundJobs;

/// <summary>
/// Configures and schedules recurring Hangfire jobs
/// </summary>
public class HangfireJobScheduler
{
    private readonly ILogger<HangfireJobScheduler> _logger;

    public HangfireJobScheduler(ILogger<HangfireJobScheduler> logger)
    {
        _logger = logger;
    }

    public void ScheduleRecurringJobs()
    {
        _logger.LogInformation("Scheduling recurring jobs");

        // Daily deadline reminder job (runs at 9 AM every day)
        RecurringJob.AddOrUpdate<ReportDeadlineReminderJob>(
            "report-deadline-reminder",
            job => job.ExecuteAsync(),
            Cron.Daily(9)); // 9 AM daily

        // Monthly archive job (runs on the 1st of each month at 2 AM)
        RecurringJob.AddOrUpdate<ArchiveOldReportsJob>(
            "archive-old-reports",
            job => job.ExecuteAsync(),
            Cron.Monthly(1, 2)); // 1st day, 2 AM

        _logger.LogInformation("Recurring jobs scheduled successfully");
    }
}
