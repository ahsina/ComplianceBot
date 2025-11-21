using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Domain.Enums;
using MediatR;

namespace ComplianceBot.Application.Reports.Queries.GetReports;

/// <summary>
/// Query to get all reports for the current tenant with filtering
/// </summary>
public record GetReportsQuery : IRequest<GetReportsResult>
{
    public ReportType? Type { get; init; }
    public ReportStatus? Status { get; init; }
    public string? ReportingPeriod { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetReportsResult
{
    public List<ReportDto> Reports { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
