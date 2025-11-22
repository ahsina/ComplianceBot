using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Application.Common.Models;
using ComplianceBot.Modules.RBE.Models;

namespace ComplianceBot.Modules.RBE.Services;

/// <summary>
/// Service for validating RBE report data according to CSSF requirements
/// Implements the standard validation service interface
/// </summary>
public class RBEValidationService : IReportValidationService<RBEReportData>
{
    public string ReportType => "RBE";

    /// <summary>
    /// Validates RBE report data according to strict CSSF requirements
    /// </summary>
    public Task<ValidationResult> ValidateAsync(RBEReportData data, CancellationToken cancellationToken = default)
    {
        var result = new ValidationResult { IsSandbox = false };

        // Validate reporting month format
        if (string.IsNullOrEmpty(data.ReportingMonth) ||
            !System.Text.RegularExpressions.Regex.IsMatch(data.ReportingMonth, @"^\d{4}-\d{2}$"))
        {
            result.AddError("ReportingMonth", "Reporting month must be in YYYY-MM format", "RBE001");
        }

        // Validate total assets
        if (data.TotalAssetsUnderManagement < 0)
        {
            result.AddError("TotalAssetsUnderManagement", "Total assets cannot be negative", "RBE002");
        }

        // Validate fund count
        if (data.NumberOfFunds < 0)
        {
            result.AddError("NumberOfFunds", "Number of funds cannot be negative", "RBE003");
        }

        // Validate client count
        if (data.NumberOfClients < 0)
        {
            result.AddError("NumberOfClients", "Number of clients cannot be negative", "RBE004");
        }

        // Validate fund positions
        if (data.FundPositions.Any())
        {
            foreach (var position in data.FundPositions)
            {
                ValidateFundPosition(position, result, isSandbox: false);
            }
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Validates RBE report data in sandbox mode with relaxed rules
    /// Issues warnings instead of errors for non-critical violations
    /// </summary>
    public Task<ValidationResult> ValidateSandboxAsync(RBEReportData data, CancellationToken cancellationToken = default)
    {
        var result = new ValidationResult { IsSandbox = true };

        // Validate reporting month format (warning only in sandbox)
        if (string.IsNullOrEmpty(data.ReportingMonth) ||
            !System.Text.RegularExpressions.Regex.IsMatch(data.ReportingMonth, @"^\d{4}-\d{2}$"))
        {
            result.AddWarning("ReportingMonth", "Reporting month should be in YYYY-MM format", "RBE001");
        }

        // Validate total assets (warning only in sandbox)
        if (data.TotalAssetsUnderManagement < 0)
        {
            result.AddWarning("TotalAssetsUnderManagement", "Total assets should not be negative", "RBE002");
        }

        // Validate fund count (warning only in sandbox)
        if (data.NumberOfFunds < 0)
        {
            result.AddWarning("NumberOfFunds", "Number of funds should not be negative", "RBE003");
        }

        // Validate client count (warning only in sandbox)
        if (data.NumberOfClients < 0)
        {
            result.AddWarning("NumberOfClients", "Number of clients should not be negative", "RBE004");
        }

        // Validate fund positions (relaxed validation)
        if (data.FundPositions.Any())
        {
            foreach (var position in data.FundPositions)
            {
                ValidateFundPosition(position, result, isSandbox: true);
            }
        }

        // Add informational message for sandbox mode
        result.AddWarning("_sandbox", "This is a sandbox validation - warnings instead of errors", "SANDBOX001");

        return Task.FromResult(result);
    }

    private void ValidateFundPosition(FundPosition position, ValidationResult result, bool isSandbox)
    {
        if (string.IsNullOrEmpty(position.FundCode))
        {
            if (isSandbox)
            {
                result.AddWarning($"FundPosition.{position.FundCode}", "Fund code should be provided", "RBE005");
            }
            else
            {
                result.AddError($"FundPosition.{position.FundCode}", "Fund code is required", "RBE005");
            }
        }

        if (position.NetAssetValue < 0)
        {
            if (isSandbox)
            {
                result.AddWarning($"FundPosition.{position.FundCode}.NetAssetValue",
                    "Net asset value should not be negative", "RBE006");
            }
            else
            {
                result.AddError($"FundPosition.{position.FundCode}.NetAssetValue",
                    "Net asset value cannot be negative", "RBE006");
            }
        }

        if (position.ValuationDate > DateTime.UtcNow)
        {
            result.AddWarning($"FundPosition.{position.FundCode}.ValuationDate",
                "Valuation date is in the future", "RBE007");
        }
    }
}
