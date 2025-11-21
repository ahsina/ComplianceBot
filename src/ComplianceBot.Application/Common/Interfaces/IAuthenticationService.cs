namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// Authentication service for generating JWT tokens
/// </summary>
public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of authentication attempt
/// </summary>
public record AuthenticationResult
{
    public bool Success { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string? ErrorMessage { get; init; }
    public UserInfo? User { get; init; }
}

/// <summary>
/// User information returned after authentication
/// </summary>
public record UserInfo
{
    public Guid UserId { get; init; }
    public Guid TenantId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public List<string> Permissions { get; init; } = new();
}
