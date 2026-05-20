using FluentValidation;
using TodoList.Application.DTOs.Auth;

namespace TodoList.API.Validation.Auth;

/// <summary>
/// Validates token rotation payloads to ensure strings are structurally sound.
/// </summary>
public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        // Access token must be present (it's the expired JWT string)
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("The expired or near-expired access token is required for rotation.");

        // Refresh token must be present
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("The tracking refresh token string is required.");
    }
}