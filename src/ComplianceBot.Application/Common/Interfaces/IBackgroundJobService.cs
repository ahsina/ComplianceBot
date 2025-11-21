namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// Service for scheduling and managing background jobs
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// Enqueue a job to run immediately in the background
    /// </summary>
    string Enqueue<T>(System.Linq.Expressions.Expression<Action<T>> methodCall);

    /// <summary>
    /// Schedule a job to run at a specific time
    /// </summary>
    string Schedule<T>(System.Linq.Expressions.Expression<Action<T>> methodCall, TimeSpan delay);

    /// <summary>
    /// Schedule a job to run at a specific date/time
    /// </summary>
    string Schedule<T>(System.Linq.Expressions.Expression<Action<T>> methodCall, DateTimeOffset enqueueAt);

    /// <summary>
    /// Add or update a recurring job
    /// </summary>
    void AddOrUpdateRecurringJob<T>(
        string jobId,
        System.Linq.Expressions.Expression<Action<T>> methodCall,
        string cronExpression);

    /// <summary>
    /// Remove a recurring job
    /// </summary>
    void RemoveRecurringJob(string jobId);

    /// <summary>
    /// Delete a job
    /// </summary>
    bool DeleteJob(string jobId);
}
