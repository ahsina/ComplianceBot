using ComplianceBot.Application.Common.Interfaces;
using Hangfire;
using System.Linq.Expressions;

namespace ComplianceBot.Infrastructure.BackgroundJobs;

/// <summary>
/// Hangfire implementation of IBackgroundJobService
/// </summary>
public class HangfireJobService : IBackgroundJobService
{
    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public void AddOrUpdateRecurringJob<T>(
        string jobId,
        Expression<Action<T>> methodCall,
        string cronExpression)
    {
        RecurringJob.AddOrUpdate(jobId, methodCall, cronExpression);
    }

    public void RemoveRecurringJob(string jobId)
    {
        RecurringJob.RemoveIfExists(jobId);
    }

    public bool DeleteJob(string jobId)
    {
        return BackgroundJob.Delete(jobId);
    }
}
