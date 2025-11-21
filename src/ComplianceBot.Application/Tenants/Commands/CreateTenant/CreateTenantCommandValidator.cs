using FluentValidation;

namespace ComplianceBot.Application.Tenants.Commands.CreateTenant;

/// <summary>
/// Validator for CreateTenantCommand
/// </summary>
public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Tenant name is required and cannot exceed 200 characters");

        RuleFor(x => x.CompanyRegistrationNumber)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Company registration number is required");

        RuleFor(x => x.Country)
            .NotEmpty()
            .Length(2)
            .WithMessage("Country must be a valid ISO 3166-1 alpha-2 code");

        RuleFor(x => x.PrimaryContactEmail)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid primary contact email is required");

        RuleFor(x => x.SubscriptionTier)
            .IsInEnum()
            .WithMessage("Invalid subscription tier");

        RuleFor(x => x.EnabledReportTypes)
            .NotEmpty()
            .WithMessage("At least one report type must be enabled");
    }
}
