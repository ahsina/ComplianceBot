using ComplianceBot.Domain.Enums;

namespace ComplianceBot.Application.Common.Services;

/// <summary>
/// Interface for generating sandbox test data
/// </summary>
public interface ISandboxDataGenerator
{
    /// <summary>
    /// Generates sample data for a specific report type
    /// </summary>
    /// <param name="reportType">The type of report to generate data for</param>
    /// <param name="tenantId">Tenant ID for the data</param>
    /// <returns>JSON string containing sample data</returns>
    Task<string> GenerateSampleDataAsync(ReportType reportType, Guid tenantId);

    /// <summary>
    /// Checks if sample data generation is supported for a report type
    /// </summary>
    /// <param name="reportType">The report type to check</param>
    /// <returns>True if supported, false otherwise</returns>
    bool IsSupported(ReportType reportType);
}
