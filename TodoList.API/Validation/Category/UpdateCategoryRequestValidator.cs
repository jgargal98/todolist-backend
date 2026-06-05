using FluentValidation;
using TodoList.Application.DTOs.Category;

namespace TodoList.API.Validation.Category;

/// <summary>
/// Validation schema rules targeting incoming records payloads for updating existing category entities.
/// </summary>
public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    /// <summary>
    /// Initializes validation rules for the category update request.
    /// </summary>
    public UpdateCategoryRequestValidator()
    {
        // Enforce the same dynamic rules for name compliance during updates
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is strictly required.")
            .MaximumLength(50).WithMessage("Category name must not exceed 50 characters.");
    }
}