namespace TodoList.Application.DTOs.Task;

/// <summary>
/// Data Transfer Object for creating a task, including its initial status, category, and subtasks payload.
/// </summary>
public class CreateTaskRequest
{
    /// <summary>Required title of the task.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Optional description providing more details.</summary>
    public string? Description { get; set; }
    /// <summary>Optional due date; must be a future date if provided.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Defaults to Pending if not explicitly provided by the client.</summary>
    public int Status { get; set; } = 1;
    /// <summary>Optional foreign key to associate the task with a category.</summary>
    public Guid? CategoryId { get; set; }

    /// <summary>Optional collection of subtasks to be embedded as JSON.</summary>
    public List<CreateSubTaskRequest> SubTasks { get; set; } = new();
}

/// <summary>
/// Nested DTO representing the initial state of a subtask.
/// </summary>
public class CreateSubTaskRequest
{
    /// <summary>Title of the subtask.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Initial completion state of the subtask.</summary>
    public bool IsDone { get; set; } = false;
}