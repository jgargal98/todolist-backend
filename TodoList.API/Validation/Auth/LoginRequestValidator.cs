using FluentValidation;
using TodoList.Application.DTOs.Auth;

namespace TodoList.API.Validation.Auth;

/// <summary>
/// Gatekeeper validator ensuring login payloads are fully populated before running database queries.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>Defines validation rules for login requests.</summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Login email is required.")
            .EmailAddress().WithMessage("Please enter a valid email address structure.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Login password cannot be left empty.");
    }
}