using ComplianceBot.Application.Common.Interfaces;
using System.Security.Claims;

namespace ComplianceBot.API.Middleware;

/// <summary>
/// Middleware to extract tenant information from JWT token
/// Sets the tenant context for the current request
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        // Extract tenant ID from JWT claims
        var tenantIdClaim = context.User?.FindFirstValue("tenant_id");

        if (Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            tenantContext.SetTenant(tenantId);
        }

        // Check if user is system admin
        var isSystemAdmin = context.User?.IsInRole("SystemAdmin") ?? false;
        if (tenantContext is Infrastructure.Identity.TenantContext tc)
        {
            tc.SetSystemAdmin(isSystemAdmin);
        }

        await _next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}
