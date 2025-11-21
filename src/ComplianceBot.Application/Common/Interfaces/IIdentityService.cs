namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// Service for managing user identities
/// </summary>
public interface IIdentityService
{
    Task<(bool Success, string UserId, string ErrorMessage)> CreateUserAsync(
        string email,
        string password,
        Guid tenantId);

    Task<bool> ValidatePasswordAsync(string email, string password);

    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    Task<bool> DeleteUserAsync(string userId);

    Task<bool> SetRefreshTokenAsync(string userId, string refreshToken, DateTime expiryTime);

    Task<(bool Valid, string UserId)> ValidateRefreshTokenAsync(string refreshToken);
}
