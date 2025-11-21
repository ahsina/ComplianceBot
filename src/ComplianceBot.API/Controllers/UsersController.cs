using ComplianceBot.Application.Identity.Commands.CreateUser;
using ComplianceBot.Application.Identity.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceBot.API.Controllers;

/// <summary>
/// API controller for managing users within a tenant
/// </summary>
[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all users for the current tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] bool? isActive)
    {
        var query = new GetUsersQuery { IsActive = isActive };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new user (TenantAdmin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "TenantAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUsers), new { id = result.Id }, result);
    }
}
