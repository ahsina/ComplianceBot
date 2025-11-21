using FluentValidation;

namespace ComplianceBot.Application.Identity.Commands.CreateUser;

/// <summary>
/// Validator for CreateUserCommand
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("First name is required and cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Last name is required and cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .WithMessage("Valid email address is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid user role");
    }
}
