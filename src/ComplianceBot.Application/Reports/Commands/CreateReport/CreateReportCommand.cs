using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Domain.Enums;
using MediatR;

namespace ComplianceBot.Application.Reports.Commands.CreateReport;

/// <summary>
/// Command to create a new report
/// </summary>
public record CreateReportCommand : IRequest<ReportDto>
{
    public ReportType Type { get; init; }
    public string ReportingPeriod { get; init; } = string.Empty;
    public int? FiscalYear { get; init; }
    public DateTime? DueDate { get; init; }
    public string? Notes { get; init; }
}
