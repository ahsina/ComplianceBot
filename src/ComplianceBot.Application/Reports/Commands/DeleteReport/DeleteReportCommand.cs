using MediatR;

namespace ComplianceBot.Application.Reports.Commands.DeleteReport;

/// <summary>
/// Command to delete a report
/// Only allowed for Draft reports
/// </summary>
public record DeleteReportCommand(Guid ReportId) : IRequest<Unit>;
