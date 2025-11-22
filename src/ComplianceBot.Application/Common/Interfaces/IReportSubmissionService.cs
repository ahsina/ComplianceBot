using ComplianceBot.Application.Common.Models;

namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// Generic interface for report submission services
/// Each reporting module (RBE, CEDR, FATCA, etc.) must implement this interface
/// </summary>
/// <typeparam name="TReportData">The specific report data model for the module</typeparam>
public interface IReportSubmissionService<TReportData> where TReportData : class
{
    /// <summary>
    /// Submits the report to the regulatory authority
    /// </summary>
    /// <param name="reportId">The report ID</param>
    /// <param name="filePath">Path to the generated report file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Submission result with reference number and status</returns>
    Task<SubmissionResult> SubmitAsync(
        Guid reportId,
        string filePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Simulates submission in sandbox mode without contacting the regulator
    /// Returns mock reference number and success status for testing
    /// </summary>
    /// <param name="reportId">The report ID</param>
    /// <param name="filePath">Path to the generated sandbox report file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Simulated submission result</returns>
    Task<SubmissionResult> SubmitSandboxAsync(
        Guid reportId,
        string filePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the report type this submitter handles
    /// </summary>
    string ReportType { get; }
}
