using Microsoft.AspNetCore.Identity;

namespace ComplianceBot.Infrastructure.Identity;

/// <summary>
/// Application user for ASP.NET Identity
/// Links to ComplianceBot.Domain.Entities.User
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Link to domain User entity
    /// </summary>
    public Guid? DomainUserId { get; set; }

    /// <summary>
    /// Tenant ID for multi-tenancy
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Refresh token for JWT
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Refresh token expiry
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
