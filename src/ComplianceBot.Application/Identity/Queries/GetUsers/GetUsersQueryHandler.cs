using AutoMapper;
using AutoMapper.QueryableExtensions;
using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComplianceBot.Application.Identity.Queries.GetUsers;

/// <summary>
/// Handler for GetUsersQuery
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(
        IApplicationDbContext context,
        ITenantContext tenantContext,
        IMapper mapper)
    {
        _context = context;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Where(u => u.TenantId == _tenantContext.TenantId);

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        return await query
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
