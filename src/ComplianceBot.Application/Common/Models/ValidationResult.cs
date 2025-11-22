namespace ComplianceBot.Application.Common.Models;

/// <summary>
/// Result of a report validation operation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indicates if validation passed without errors
    /// </summary>
    public bool IsValid => !Errors.Any();

    /// <summary>
    /// List of validation errors (blocking issues)
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings (non-blocking issues)
    /// </summary>
    public List<ValidationError> Warnings { get; set; } = new();

    /// <summary>
    /// Indicates if this is a sandbox validation result
    /// </summary>
    public bool IsSandbox { get; set; }

    /// <summary>
    /// Adds an error to the validation result
    /// </summary>
    public void AddError(string field, string message, string? errorCode = null)
    {
        Errors.Add(new ValidationError
        {
            Field = field,
            Message = message,
            ErrorCode = errorCode,
            Severity = ValidationSeverity.Error
        });
    }

    /// <summary>
    /// Adds a warning to the validation result
    /// </summary>
    public void AddWarning(string field, string message, string? errorCode = null)
    {
        Warnings.Add(new ValidationError
        {
            Field = field,
            Message = message,
            ErrorCode = errorCode,
            Severity = ValidationSeverity.Warning
        });
    }
}

/// <summary>
/// Validation error or warning details
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Field or property that failed validation
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error code (e.g., "RBE001", "CEDR042")
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Severity level
    /// </summary>
    public ValidationSeverity Severity { get; set; }
}

/// <summary>
/// Severity of validation issue
/// </summary>
public enum ValidationSeverity
{
    /// <summary>
    /// Informational message
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning (non-blocking)
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error (blocking)
    /// </summary>
    Error = 2
}
