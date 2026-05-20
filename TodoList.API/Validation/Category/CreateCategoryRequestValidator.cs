using FluentValidation;
using TodoList.Application.DTOs.Category;

namespace TodoList.API.Validation.Category;

/// <summary>
/// Validation schema rules targeting incoming payloads for creating category entities.
/// </summary>
public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    /// <summary>
    /// Initializes validation rules for the category creation request.
    /// </summary>
    public CreateCategoryRequestValidator()
    {
        // Enforce that the name cannot be null, empty, or just whitespace
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is strictly required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
    }
}