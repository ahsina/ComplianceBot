using ComplianceBot.Modules.RBE.Models;

namespace ComplianceBot.Modules.RBE.Services;

/// <summary>
/// Service for validating RBE report data
/// </summary>
public class RBEValidationService
{
    /// <summary>
    /// Validates RBE report data according to CSSF requirements
    /// </summary>
    public ValidationResult Validate(RBEReportData data)
    {
        var result = new ValidationResult();

        // Validate reporting month format
        if (string.IsNullOrEmpty(data.ReportingMonth) ||
            !System.Text.RegularExpressions.Regex.IsMatch(data.ReportingMonth, @"^\d{4}-\d{2}$"))
        {
            result.AddError("ReportingMonth", "Reporting month must be in YYYY-MM format");
        }

        // Validate total assets
        if (data.TotalAssetsUnderManagement < 0)
        {
            result.AddError("TotalAssetsUnderManagement", "Total assets cannot be negative");
        }

        // Validate fund count
        if (data.NumberOfFunds < 0)
        {
            result.AddError("NumberOfFunds", "Number of funds cannot be negative");
        }

        // Validate client count
        if (data.NumberOfClients < 0)
        {
            result.AddError("NumberOfClients", "Number of clients cannot be negative");
        }

        // Validate fund positions
        if (data.FundPositions.Any())
        {
            foreach (var position in data.FundPositions)
            {
                ValidateFundPosition(position, result);
            }
        }

        return result;
    }

    private void ValidateFundPosition(FundPosition position, ValidationResult result)
    {
        if (string.IsNullOrEmpty(position.FundCode))
        {
            result.AddError($"FundPosition.{position.FundCode}", "Fund code is required");
        }

        if (position.NetAssetValue < 0)
        {
            result.AddError($"FundPosition.{position.FundCode}.NetAssetValue",
                "Net asset value cannot be negative");
        }

        if (position.ValuationDate > DateTime.UtcNow)
        {
            result.AddWarning($"FundPosition.{position.FundCode}.ValuationDate",
                "Valuation date is in the future");
        }
    }
}

/// <summary>
/// Result of validation
/// </summary>
public class ValidationResult
{
    public List<ValidationMessage> Errors { get; } = new();
    public List<ValidationMessage> Warnings { get; } = new();

    public bool IsValid => !Errors.Any();

    public void AddError(string field, string message)
    {
        Errors.Add(new ValidationMessage { Field = field, Message = message });
    }

    public void AddWarning(string field, string message)
    {
        Warnings.Add(new ValidationMessage { Field = field, Message = message });
    }
}

public class ValidationMessage
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
