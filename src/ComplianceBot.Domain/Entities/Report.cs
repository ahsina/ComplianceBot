using ComplianceBot.Domain.Common;
using ComplianceBot.Domain.Enums;

namespace ComplianceBot.Domain.Entities;

/// <summary>
/// Report entity - represents a compliance report
/// </summary>
public class Report : TenantEntity
{
    public ReportType Type { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Draft;

    /// <summary>
    /// Indicates if this is a sandbox/test report (no real submission)
    /// </summary>
    public bool IsSandbox { get; set; } = false;

    /// <summary>
    /// Reporting period (e.g., "2024-01" for RBE January 2024)
    /// </summary>
    public string ReportingPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Fiscal year for annual reports
    /// </summary>
    public int? FiscalYear { get; set; }

    /// <summary>
    /// Regulatory deadline for submission
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// When the report was submitted
    /// </summary>
    public DateTime? SubmittedAt { get; set; }

    /// <summary>
    /// User who submitted the report
    /// </summary>
    public Guid? SubmittedByUserId { get; set; }

    /// <summary>
    /// Reference number from regulator after submission
    /// </summary>
    public string? RegulatoryReferenceNumber { get; set; }

    /// <summary>
    /// Validation errors/warnings
    /// </summary>
    public string? ValidationResult { get; set; }

    /// <summary>
    /// Notes/comments from compliance team
    /// </summary>
    public string? Notes { get; set; }

    // Navigation properties
    public User? SubmittedByUser { get; set; }
    public ICollection<ReportDocument> Documents { get; set; } = new List<ReportDocument>();
}
