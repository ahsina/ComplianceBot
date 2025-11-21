using ComplianceBot.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ComplianceBot.Infrastructure.Identity;

/// <summary>
/// Implementation of ICurrentUserService
/// Extracts user information from JWT claims
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public Guid? TenantId
    {
        get
        {
            var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("tenant_id");
            return Guid.TryParse(tenantIdClaim, out var tenantId) ? tenantId : null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsSystemAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole("SystemAdmin") ?? false;

    public IEnumerable<string> Permissions
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindAll("permissions")
                .Select(c => c.Value) ?? Enumerable.Empty<string>();
        }
    }
}
