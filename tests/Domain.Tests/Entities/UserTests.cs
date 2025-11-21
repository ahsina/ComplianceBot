using ComplianceBot.Domain.Entities;
using ComplianceBot.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ComplianceBot.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void FullName_CombinesFirstAndLastName()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Fact]
    public void User_CanBeCreatedWithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Should().NotBeNull();
        user.IsActive.Should().BeTrue();
        user.FirstName.Should().BeEmpty();
        user.LastName.Should().BeEmpty();
        user.Email.Should().BeEmpty();
    }

    [Fact]
    public void User_CanSetRole()
    {
        // Arrange
        var user = new User();

        // Act
        user.Role = UserRole.ComplianceOfficer;

        // Assert
        user.Role.Should().Be(UserRole.ComplianceOfficer);
    }

    [Fact]
    public void User_CanTrackLastLogin()
    {
        // Arrange
        var user = new User();
        var loginTime = DateTime.UtcNow;

        // Act
        user.LastLoginAt = loginTime;

        // Assert
        user.LastLoginAt.Should().Be(loginTime);
    }
}
