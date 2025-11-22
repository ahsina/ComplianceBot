namespace ComplianceBot.Application.Common.Models;

/// <summary>
/// Result of a report submission operation
/// </summary>
public class SubmissionResult
{
    /// <summary>
    /// Indicates if submission was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Reference number from the regulatory authority
    /// For sandbox: mock reference like "SANDBOX-{Guid}"
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Status message from the regulator
    /// </summary>
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Indicates if this was a sandbox submission (simulated)
    /// </summary>
    public bool IsSandbox { get; set; }

    /// <summary>
    /// Error message if submission failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// When the submission was made
    /// </summary>
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional metadata from the submission
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}
