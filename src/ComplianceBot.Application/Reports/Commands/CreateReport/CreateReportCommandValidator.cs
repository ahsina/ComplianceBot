using FluentValidation;

namespace ComplianceBot.Application.Reports.Commands.CreateReport;

/// <summary>
/// Validator for CreateReportCommand
/// </summary>
public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
{
    public CreateReportCommandValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid report type");

        RuleFor(x => x.ReportingPeriod)
            .NotEmpty()
            .WithMessage("Reporting period is required")
            .MaximumLength(50)
            .WithMessage("Reporting period cannot exceed 50 characters");

        RuleFor(x => x.FiscalYear)
            .GreaterThan(2000)
            .LessThanOrEqualTo(2100)
            .When(x => x.FiscalYear.HasValue)
            .WithMessage("Fiscal year must be between 2000 and 2100");

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 2000 characters");
    }
}
