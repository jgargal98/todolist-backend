using FluentValidation;
using TodoList.Application.DTOs.Task;

namespace TodoList.API.Validation;

/// <summary>
/// Enforces business data integrity rules and payload constraints for updating task entities.
/// </summary>
public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        // 1. Title Constraints
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required and cannot be empty.")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters.");

        // 2. Description Constraints
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Task description cannot exceed 1000 characters.");

        // 3. Status Constraints
        RuleFor(x => x.Status)
            .Must(status => status >= 1 && status <= 5)
            .WithMessage("Task status must be a valid integer code between 1 and 5.");

        // 4. Due Date Constraints
        RuleFor(x => x.DueDate)
            .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
            .WithMessage("The specified task due date must be set in the future.");
    }
}