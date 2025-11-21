using ComplianceBot.Application.Common.Interfaces;
using ComplianceBot.Domain.Entities;
using ComplianceBot.Domain.Enums;
using ComplianceBot.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace ComplianceBot.IntegrationTests.Controllers;

public class ReportsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ReportsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetReports_WithoutAuth_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/reports");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateReport_WithValidData_ReturnsCreated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createReportRequest = new
        {
            type = (int)ReportType.RBE,
            reportingPeriod = "2024-01",
            fiscalYear = 2024,
            dueDate = DateTime.UtcNow.AddDays(30),
            notes = "Test report"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/reports", createReportRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetReports_WithAuth_ReturnsOk()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/reports");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        // Seed test data
        await SeedTestDataAsync();

        // Login
        var loginRequest = new
        {
            email = "test@example.com",
            password = "Test@123456"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        return result?.AccessToken ?? throw new Exception("Failed to get auth token");
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

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }

    private class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
