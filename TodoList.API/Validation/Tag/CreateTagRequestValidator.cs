using FluentValidation;
using TodoList.Application.DTOs.Tag;

namespace TodoList.Application.Validators.Tag;

/// <summary>
/// Provides explicit strict validation routine rules for incoming tag instantiation requests.
/// </summary>
public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The tag descriptive name cannot be empty.")
            .MaximumLength(50).WithMessage("The tag descriptive name cannot exceed 50 characters.");
    }
}