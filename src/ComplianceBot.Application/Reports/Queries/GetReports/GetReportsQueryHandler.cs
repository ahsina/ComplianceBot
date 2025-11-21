using AutoMapper;
using AutoMapper.QueryableExtensions;
using ComplianceBot.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceBot.Application.Reports.Queries.GetReports;

/// <summary>
/// Handler for GetReportsQuery
/// </summary>
public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, GetReportsResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public GetReportsQueryHandler(
        IApplicationDbContext context,
        ITenantContext tenantContext,
        IMapper mapper)
    {
        _context = context;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<GetReportsResult> Handle(GetReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Reports
            .Where(r => r.TenantId == _tenantContext.TenantId);

        // Apply filters
        if (request.Type.HasValue)
        {
            query = query.Where(r => r.Type == request.Type.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        if (!string.IsNullOrEmpty(request.ReportingPeriod))
        {
            query = query.Where(r => r.ReportingPeriod == request.ReportingPeriod);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var reports = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ComplianceBot.Application.Common.DTOs.ReportDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new GetReportsResult
        {
            Reports = reports,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
