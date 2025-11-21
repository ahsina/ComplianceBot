using ComplianceBot.Domain.Entities;
using ComplianceBot.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ComplianceBot.Domain.Tests.Entities;

public class TenantTests
{
    [Fact]
    public void DatabaseSchema_ReturnsCorrectFormat()
    {
        // Arrange
        var tenantId = Guid.Parse("12345678-1234-1234-1234-123456789012");
        var tenant = new Tenant { Id = tenantId };

        // Act
        var schema = tenant.DatabaseSchema;

        // Assert
        schema.Should().Be("tenant_12345678123412341234123456789012");
        schema.Should().StartWith("tenant_");
        schema.Should().NotContain("-");
    }

    [Fact]
    public void Tenant_CanBeCreatedWithDefaultValues()
    {
        // Arrange & Act
        var tenant = new Tenant();

        // Assert
        tenant.Should().NotBeNull();
        tenant.Country.Should().Be("LU");
        tenant.IsActive.Should().BeTrue();
        tenant.EnabledReportTypes.Should().NotBeNull().And.BeEmpty();
        tenant.Users.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Tenant_CanAddEnabledReportTypes()
    {
        // Arrange
        var tenant = new Tenant();

        // Act
        tenant.EnabledReportTypes.Add(ReportType.RBE);
        tenant.EnabledReportTypes.Add(ReportType.CEDR);

        // Assert
        tenant.EnabledReportTypes.Should().HaveCount(2);
        tenant.EnabledReportTypes.Should().Contain(ReportType.RBE);
        tenant.EnabledReportTypes.Should().Contain(ReportType.CEDR);
    }

    [Fact]
    public void Tenant_CanSetSubscriptionInformation()
    {
        // Arrange
        var tenant = new Tenant();
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddYears(1);

        // Act
        tenant.SubscriptionTier = SubscriptionTier.Professional;
        tenant.SubscriptionStartDate = startDate;
        tenant.SubscriptionEndDate = endDate;

        // Assert
        tenant.SubscriptionTier.Should().Be(SubscriptionTier.Professional);
        tenant.SubscriptionStartDate.Should().Be(startDate);
        tenant.SubscriptionEndDate.Should().Be(endDate);
    }
}
