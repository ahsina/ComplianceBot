using ComplianceBot.Domain.Enums;
using MediatR;

namespace ComplianceBot.Application.Reports.Commands.UpdateReportStatus;

/// <summary>
/// Command to update report status
/// </summary>
public record UpdateReportStatusCommand : IRequest<Unit>
{
    public Guid ReportId { get; init; }
    public ReportStatus NewStatus { get; init; }
    public string? Notes { get; init; }
}
