using ComplianceBot.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ComplianceBot.Infrastructure.Identity;

/// <summary>
/// Implementation of IIdentityService using ASP.NET Identity
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Success, string UserId, string ErrorMessage)> CreateUserAsync(
        string email,
        string password,
        Guid tenantId)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true, // Auto-confirm in ComplianceBot
            TenantId = tenantId,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return (true, user.Id, string.Empty);
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return (false, string.Empty, errors);
    }

    public async Task<bool> ValidatePasswordAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || !user.IsActive)
        {
            return false;
        }

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        // Soft delete by marking as inactive
        user.IsActive = false;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<bool> SetRefreshTokenAsync(string userId, string refreshToken, DateTime expiryTime)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = expiryTime;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<(bool Valid, string UserId)> ValidateRefreshTokenAsync(string refreshToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null ||
            !user.IsActive ||
            user.RefreshTokenExpiryTime == null ||
            user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return (false, string.Empty);
        }

        return (true, user.Id);
    }
}
