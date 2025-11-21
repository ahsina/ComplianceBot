namespace ComplianceBot.Domain.Enums;

/// <summary>
/// Status lifecycle of a compliance report
/// </summary>
public enum ReportStatus
{
    /// <summary>
    /// Report is being drafted
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Report is being validated
    /// </summary>
    Validating = 2,

    /// <summary>
    /// Validation passed, ready for submission
    /// </summary>
    Validated = 3,

    /// <summary>
    /// Validation failed, requires correction
    /// </summary>
    ValidationFailed = 4,

    /// <summary>
    /// Report is being submitted to regulator
    /// </summary>
    Submitting = 5,

    /// <summary>
    /// Successfully submitted to regulator
    /// </summary>
    Submitted = 6,

    /// <summary>
    /// Submission failed
    /// </summary>
    SubmissionFailed = 7,

    /// <summary>
    /// Acknowledged by regulator
    /// </summary>
    Acknowledged = 8,

    /// <summary>
    /// Rejected by regulator
    /// </summary>
    Rejected = 9,

    /// <summary>
    /// Archived (historical record)
    /// </summary>
    Archived = 10
}
