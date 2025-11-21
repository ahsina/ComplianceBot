using ComplianceBot.Application.Common.DTOs;
using MediatR;

namespace ComplianceBot.Application.Reports.Queries.GetReport;

/// <summary>
/// Query to get a single report by ID
/// </summary>
public record GetReportQuery(Guid ReportId) : IRequest<ReportDto>;
