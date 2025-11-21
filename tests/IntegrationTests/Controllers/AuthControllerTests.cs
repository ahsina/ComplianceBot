using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Entities;
using ComplianceBot.Domain.Enums;
using ComplianceBot.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ComplianceBot.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        await SeedTestDataAsync();

        var loginRequest = new
        {
            email = "test@example.com",
            password = "Test@123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            email = "invalid@example.com",
            password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ComplianceBotDbContext>();
        var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();

        // Create test tenant
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            CompanyRegistrationNumber = "TEST123",
            VATNumber = "TEST123",
            Country = "LU",
            City = "Luxembourg",
            Address = "Test Address",
            PostalCode = "L-1234",
            PrimaryContactName = "Test Contact",
            PrimaryContactEmail = "contact@test.com",
            PrimaryContactPhone = "+352 123 456",
            SubscriptionTier = SubscriptionTier.Professional,
            SubscriptionStartDate = DateTime.UtcNow,
            IsActive = true,
            EnabledReportTypes = new List<ReportType> { ReportType.RBE },
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        // Create identity user
        var (success, identityId, _) = await identityService.CreateUserAsync(
            "test@example.com",
            "Test@123456",
            tenant.Id);

        if (success)
        {
            // Create domain user (linked to identity)
            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PhoneNumber = "+352 123 456",
                Role = UserRole.ComplianceOfficer,
                IdentityId = identityId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            // Note: This would normally be in tenant schema, but in-memory DB doesn't support schemas
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}
