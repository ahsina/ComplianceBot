using ComplianceBot.Application.Common.Models;

namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// Generic interface for report generation services
/// Each reporting module (RBE, CEDR, FATCA, etc.) must implement this interface
/// </summary>
/// <typeparam name="TReportData">The specific report data model for the module</typeparam>
public interface IReportGenerationService<TReportData> where TReportData : class
{
    /// <summary>
    /// Generates the regulatory report file (XML, CSV, etc.)
    /// </summary>
    /// <param name="reportId">The report ID</param>
    /// <param name="data">The validated report data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated file information (path, format, size)</returns>
    Task<ReportFileResult> GenerateAsync(
        Guid reportId,
        TReportData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a sandbox/test report file with watermark
    /// File is marked as "SANDBOX - NOT FOR SUBMISSION"
    /// </summary>
    /// <param name="reportId">The report ID</param>
    /// <param name="data">The validated report data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated sandbox file information</returns>
    Task<ReportFileResult> GenerateSandboxAsync(
        Guid reportId,
        TReportData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the report type this generator handles
    /// </summary>
    string ReportType { get; }
}
