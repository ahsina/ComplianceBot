using ComplianceBot.Domain.Enums;

namespace ComplianceBot.Application.Common.DTOs;

/// <summary>
/// Data transfer object for Report
/// </summary>
public class ReportDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public ReportType Type { get; set; }
    public ReportStatus Status { get; set; }
    public string ReportingPeriod { get; set; } = string.Empty;
    public int? FiscalYear { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string? SubmittedByUserName { get; set; }
    public string? RegulatoryReferenceNumber { get; set; }
    public string? ValidationResult { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
