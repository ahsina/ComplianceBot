using AutoMapper;
using AutoMapper.QueryableExtensions;
using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceBot.Application.Tenants.Queries.GetTenant;

/// <summary>
/// Handler for GetTenantQuery
/// </summary>
public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, TenantDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTenantQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TenantDto> Handle(GetTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenants
            .Where(t => t.Id == request.TenantId)
            .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant == null)
        {
            throw new EntityNotFoundException(nameof(ComplianceBot.Domain.Entities.Tenant), request.TenantId);
        }

        return tenant;
    }
}
