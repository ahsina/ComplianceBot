using ComplianceBot.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceBot.API.Controllers;

/// <summary>
/// API controller for authentication
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Authenticate user and get access token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authenticationService.AuthenticateAsync(request.Email, request.Password);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        return Ok(new
        {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken,
            expiresAt = result.ExpiresAt,
            user = result.User
        });
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authenticationService.RefreshTokenAsync(request.RefreshToken);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        return Ok(new
        {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken,
            expiresAt = result.ExpiresAt
        });
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCurrentUser([FromServices] ICurrentUserService currentUser)
    {
        return Ok(new
        {
            userId = currentUser.UserId,
            email = currentUser.Email,
            tenantId = currentUser.TenantId,
            isAuthenticated = currentUser.IsAuthenticated,
            isSystemAdmin = currentUser.IsSystemAdmin,
            permissions = currentUser.Permissions
        });
    }
}

/// <summary>
/// Login request model
/// </summary>
public record LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Refresh token request model
/// </summary>
public record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
