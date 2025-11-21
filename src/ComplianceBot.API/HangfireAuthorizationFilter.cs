using Hangfire.Dashboard;

namespace ComplianceBot.API;

/// <summary>
/// Authorization filter for Hangfire Dashboard
/// In development, allows all access. In production, should check for admin role.
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In development, allow all access
        // In production, you should check if user is authenticated and has admin role
        var httpContext = context.GetHttpContext();

        // TODO: In production, add proper authorization
        // return httpContext.User.Identity?.IsAuthenticated == true &&
        //        httpContext.User.IsInRole("SystemAdmin");

        return true; // Allow all for now (development only!)
    }
}
