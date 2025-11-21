using ComplianceBot.Application.Reports.Commands.CreateReport;
using ComplianceBot.Domain.Enums;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ComplianceBot.Application.Tests.Reports.Commands;

public class CreateReportCommandValidatorTests
{
    private readonly CreateReportCommandValidator _validator;

    public CreateReportCommandValidatorTests()
    {
        _validator = new CreateReportCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateReportCommand
        {
            Type = ReportType.RBE,
            ReportingPeriod = "2024-01",
            FiscalYear = 2024,
            DueDate = DateTime.UtcNow.AddDays(30),
            Notes = "Test report notes"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.Should NotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyReportingPeriod_ShouldHaveError()
    {
        // Arrange
        var command = new CreateReportCommand
        {
            Type = ReportType.RBE,
            ReportingPeriod = "",
            FiscalYear = 2024
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReportingPeriod);
    }

    [Fact]
    public void Validate_ReportingPeriodTooLong_ShouldHaveError()
    {
        // Arrange
        var command = new CreateReportCommand
        {
            Type = ReportType.RBE,
            ReportingPeriod = new string('A', 51), // 51 characters
            FiscalYear = 2024
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReportingPeriod);
    }

    [Theory]
    [InlineData(1999)]
    [InlineData(2101)]
    public void Validate_InvalidFiscalYear_ShouldHaveError(int fiscalYear)
    {
        // Arrange
        var command = new CreateReportCommand
        {
            Type = ReportType.RBE,
            ReportingPeriod = "2024-01",
            FiscalYear = fiscalYear
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FiscalYear);
    }

    [Fact]
    public void Validate_NotesTooLong_ShouldHaveError()
    {
        // Arrange
        var command = new CreateReportCommand
        {
            Type = ReportType.RBE,
            ReportingPeriod = "2024-01",
            Notes = new string('A', 2001) // 2001 characters
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Notes);
    }
}
