using FluentValidation;
using TodoList.Application.DTOs.Auth;

namespace TodoList.API.Validation.Auth;
/// <summary>
/// Enforces data integrity, credentials formatting, and password complexity criteria during registration.
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        // 1. Email Constraints
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is strictly required.")
            .EmailAddress().WithMessage("A valid email address format is required (e.g., user@example.com).")
            .MaximumLength(150).WithMessage("Email address cannot exceed 150 characters.");

        // 2. Password Complexity Constraints
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(64).WithMessage("Password cannot exceed 64 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one numeric digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character (e.g., @, #, $, %).");

        // 3. Cross-Field Validation: Password Matching
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("The confirmation password does not match the chosen password.");
    }
}