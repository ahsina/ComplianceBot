using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Infrastructure.Files;
using ComplianceBot.Infrastructure.Identity;
using ComplianceBot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ComplianceBot.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ComplianceBotDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ComplianceBotDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ComplianceBotDbContext>());

        // Multi-tenancy
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Authentication
        services.AddSingleton<JwtTokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        // File storage
        services.AddSingleton<IFileStorageService, AzureBlobStorageService>();

        // HTTP Context Accessor (needed for CurrentUserService)
        services.AddHttpContextAccessor();

        return services;
    }
}
