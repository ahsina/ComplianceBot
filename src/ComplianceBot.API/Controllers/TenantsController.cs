using ComplianceBot.Application.Tenants.Commands.CreateTenant;
using ComplianceBot.Application.Tenants.Queries.GetTenant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceBot.API.Controllers;

/// <summary>
/// API controller for managing tenants
/// Most endpoints restricted to SystemAdmin role
/// </summary>
[ApiController]
[Route("api/v1/tenants")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get tenant information
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenant(Guid id)
    {
        var query = new GetTenantQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new tenant (SystemAdmin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SystemAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTenant), new { id = result.Id }, result);
    }
}
