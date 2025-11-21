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
    private readonly IIdentityService _identityService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IApplicationDbContext context,
        IIdentityService identityService,
        JwtTokenService jwtTokenService,
        ILogger<AuthenticationService> logger)
    {
        _context = context;
        _identityService = identityService;
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

        // Verify password with Identity provider
        var passwordValid = await _identityService.ValidatePasswordAsync(email, password);

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
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); // 7 days

        // Store refresh token in Identity database
        await _identityService.SetRefreshTokenAsync(user.IdentityId, refreshToken, refreshTokenExpiry);

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
        // Validate refresh token
        var (valid, identityUserId) = await _identityService.ValidateRefreshTokenAsync(refreshToken);

        if (!valid)
        {
            _logger.LogWarning("Invalid refresh token");
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = "Invalid or expired refresh token"
            };
        }

        // Find user by identity ID
        var user = await _context.Users
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.IdentityId == identityUserId, cancellationToken);

        if (user == null || !user.IsActive || user.Tenant == null || !user.Tenant.IsActive)
        {
            _logger.LogWarning("Refresh token for inactive user or tenant");
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = "Account is not active"
            };
        }

        // Generate new tokens
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

        var newAccessToken = _jwtTokenService.GenerateAccessToken(userInfo);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(60);
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // Rotate refresh token
        await _identityService.SetRefreshTokenAsync(identityUserId, newRefreshToken, refreshTokenExpiry);

        _logger.LogInformation("Token refreshed for user: {Email}", user.Email);

        return new AuthenticationResult
        {
            Success = true,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = expiresAt,
            User = userInfo
        };
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
