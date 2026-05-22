namespace TodoList.Application.DTOs.Task;

/// <summary>
/// Data Transfer Object containing the allowed updatable fields for a task, including its embedded subtasks.
/// </summary>
public record UpdateTaskRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    int Status,
    Guid? CategoryId,
    List<UpdateSubTaskRequest> SubTasks,
    List<Guid> TagIds
);

/// <summary>
/// Payload contract for mutating an individual subtask item state.
/// </summary>
public class UpdateSubTaskRequest
{
    /// <summary>Updated title of the subtask.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Updated completion state of the subtask.</summary>
    public bool IsDone { get; set; } = false;
}