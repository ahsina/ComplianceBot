using AutoMapper;
using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Entities;
using MediatR;

namespace ComplianceBot.Application.Tenants.Commands.CreateTenant;

/// <summary>
/// Handler for CreateTenantCommand
/// Creates a new tenant and initializes their database schema
/// </summary>
public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public CreateTenantCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CompanyRegistrationNumber = request.CompanyRegistrationNumber,
            VATNumber = request.VATNumber,
            Country = request.Country,
            City = request.City,
            Address = request.Address,
            PostalCode = request.PostalCode,
            PrimaryContactName = request.PrimaryContactName,
            PrimaryContactEmail = request.PrimaryContactEmail,
            PrimaryContactPhone = request.PrimaryContactPhone,
            SubscriptionTier = request.SubscriptionTier,
            SubscriptionStartDate = DateTime.UtcNow,
            IsActive = true,
            EnabledReportTypes = request.EnabledReportTypes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.Email
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Create tenant database schema
        // await _databaseService.CreateTenantSchemaAsync(tenant.DatabaseSchema);

        return _mapper.Map<TenantDto>(tenant);
    }
}
