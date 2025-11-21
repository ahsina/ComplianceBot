using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComplianceBot.Infrastructure.Identity;

/// <summary>
/// Implementation of IAuthenticationService
/// Handles user authentication and JWT token generation
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IApplicationDbContext _context;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IApplicationDbContext context,
        JwtTokenService jwtTokenService,
        ILogger<AuthenticationService> logger)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        // Find user by email across all tenants
        var user = await _context.Users
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Authentication failed for email: {Email}", email);
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = "Invalid email or password"
            };
        }

        // Check if tenant is active
        if (user.Tenant == null || !user.Tenant.IsActive)
        {
            _logger.LogWarning("Authentication failed - tenant inactive for user: {Email}", email);
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = "Account is not active"
            };
        }

        // TODO: Verify password with Identity provider
        // For now, this is a placeholder
        var passwordValid = await VerifyPasswordAsync(user.IdentityId, password);

        if (!passwordValid)
        {
            _logger.LogWarning("Invalid password for email: {Email}", email);
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = "Invalid email or password"
            };
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var userInfo = new UserInfo
        {
            UserId = user.Id,
            TenantId = user.TenantId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            Permissions = GetUserPermissions(user.Role)
        };

        var accessToken = _jwtTokenService.GenerateAccessToken(userInfo);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        // TODO: Store refresh token in database

        _logger.LogInformation("User authenticated successfully: {Email}", email);

        return new AuthenticationResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = userInfo
        };
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement refresh token validation and rotation
        await Task.CompletedTask;

        return new AuthenticationResult
        {
            Success = false,
            ErrorMessage = "Refresh token functionality not implemented yet"
        };
    }

    private async Task<bool> VerifyPasswordAsync(string identityId, string password)
    {
        // TODO: Implement password verification with Identity provider
        // This is a placeholder - never use in production!
        await Task.CompletedTask;
        return true; // Placeholder
    }

    private List<string> GetUserPermissions(UserRole role)
    {
        return role switch
        {
            UserRole.SystemAdmin => new List<string>
            {
                "tenants:create", "tenants:read", "tenants:update", "tenants:delete",
                "users:create", "users:read", "users:update", "users:delete",
                "reports:create", "reports:read", "reports:update", "reports:delete", "reports:submit"
            },
            UserRole.TenantAdmin => new List<string>
            {
                "users:create", "users:read", "users:update",
                "reports:create", "reports:read", "reports:update", "reports:delete", "reports:submit"
            },
            UserRole.ComplianceOfficer => new List<string>
            {
                "reports:create", "reports:read", "reports:update", "reports:submit"
            },
            UserRole.ComplianceAnalyst => new List<string>
            {
                "reports:create", "reports:read", "reports:update"
            },
            UserRole.Viewer => new List<string>
            {
                "reports:read"
            },
            _ => new List<string>()
        };
    }
}
