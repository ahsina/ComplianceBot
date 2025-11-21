using AutoMapper;
using AutoMapper.QueryableExtensions;
using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceBot.Application.Reports.Queries.GetReport;

/// <summary>
/// Handler for GetReportQuery
/// </summary>
public class GetReportQueryHandler : IRequestHandler<GetReportQuery, ReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public GetReportQueryHandler(
        IApplicationDbContext context,
        ITenantContext tenantContext,
        IMapper mapper)
    {
        _context = context;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<ReportDto> Handle(GetReportQuery request, CancellationToken cancellationToken)
    {
        var report = await _context.Reports
            .Where(r => r.Id == request.ReportId && r.TenantId == _tenantContext.TenantId)
            .ProjectTo<ReportDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (report == null)
        {
            throw new EntityNotFoundException(nameof(Report), request.ReportId);
        }

        return report;
    }
}
