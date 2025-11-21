using MediatR;

namespace ComplianceBot.Application.Reports.Commands.SubmitReport;

/// <summary>
/// Command to submit a report to the regulator
/// </summary>
public record SubmitReportCommand(Guid ReportId) : IRequest<SubmitReportResult>;

public record SubmitReportResult
{
    public Guid ReportId { get; init; }
    public string RegulatoryReferenceNumber { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
