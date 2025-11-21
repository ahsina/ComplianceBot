using AutoMapper;
using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Entities;
using ComplianceBot.Domain.Exceptions;
using MediatR;

namespace ComplianceBot.Application.Identity.Commands.CreateUser;

/// <summary>
/// Handler for CreateUserCommand
/// Creates a new user and their identity
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(
        IApplicationDbContext context,
        IIdentityService identityService,
        ITenantContext tenantContext,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _identityService = identityService;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Create identity user with password hashing
        var (success, identityId, errorMessage) = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            _tenantContext.TenantId);

        if (!success)
        {
            throw new ValidationException(errorMessage);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantContext.TenantId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            IdentityId = identityId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}
