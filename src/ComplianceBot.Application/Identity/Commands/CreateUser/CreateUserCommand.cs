using ComplianceBot.Application.Common.DTOs;
using ComplianceBot.Domain.Enums;
using MediatR;

namespace ComplianceBot.Application.Identity.Commands.CreateUser;

/// <summary>
/// Command to create a new user within a tenant
/// </summary>
public record CreateUserCommand : IRequest<UserDto>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public string Password { get; init; } = string.Empty;
}
