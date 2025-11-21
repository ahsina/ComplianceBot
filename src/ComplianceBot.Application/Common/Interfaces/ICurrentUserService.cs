namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// Provides current user information from JWT token
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    Guid? TenantId { get; }
    bool IsAuthenticated { get; }
    bool IsSystemAdmin { get; }
    IEnumerable<string> Permissions { get; }
}
