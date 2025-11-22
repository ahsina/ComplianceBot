using ComplianceBot.Application.Common.Models;

namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// Generic interface for report validation services
/// Each reporting module (RBE, CEDR, FATCA, etc.) must implement this interface
/// </summary>
/// <typeparam name="TReportData">The specific report data model for the module</typeparam>
public interface IReportValidationService<TReportData> where TReportData : class
{
    /// <summary>
    /// Validates report data according to regulatory requirements
    /// </summary>
    /// <param name="data">The report data to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with errors and warnings</returns>
    Task<ValidationResult> ValidateAsync(
        TReportData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates report data in sandbox mode with relaxed rules
    /// Warnings are issued instead of blocking errors for testing purposes
    /// </summary>
    /// <param name="data">The report data to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with warnings instead of errors</returns>
    Task<ValidationResult> ValidateSandboxAsync(
        TReportData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the report type this validator handles
    /// </summary>
    string ReportType { get; }
}
