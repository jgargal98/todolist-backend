using FluentValidation;
using TodoList.Application.DTOs.Task;

namespace TodoList.API.Validation.Task;

/// <summary>
/// Strongly-typed validator enforcing business rules on the CreateTaskRequest DTO payload.
/// </summary>
public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        // Rule for Title: Cannot be empty, must be trimmed implicitly, max 100 characters
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is strictly required.")
            .MaximumLength(100).WithMessage("Task title cannot exceed 100 characters.");

        // Rule for Status: Bounded strictly between 1 and 5
        RuleFor(x => x.Status)
            .InclusiveBetween(1, 5).WithMessage("Status value must fall within the 1 to 5 range.");

        // Rule for DueDate: Must represent a future point in time if provided
        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("The task due date must be set in the future.")
            .When(x => x.DueDate.HasValue);

        // Rule for Child Collection: Validates each item inside the subtask list
        RuleForEach(x => x.SubTasks).ChildRules(subTask =>
        {
            subTask.RuleFor(st => st.Title)
                .NotEmpty().WithMessage("Every individual subtask must contain a valid title.");
        });
    }
}