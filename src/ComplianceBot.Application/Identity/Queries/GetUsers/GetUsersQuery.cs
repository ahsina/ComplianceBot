using ComplianceBot.Application.Common.DTOs;
using MediatR;

namespace ComplianceBot.Application.Identity.Queries.GetUsers;

/// <summary>
/// Query to get all users for the current tenant
/// </summary>
public record GetUsersQuery : IRequest<List<UserDto>>
{
    public bool? IsActive { get; init; }
}
