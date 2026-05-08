namespace TodoList.Domain.Entities;

/// <summary>
/// Represents the possible states of a task according to the ERD.
/// </summary>
public enum TaskStatus
{
    Pending = 1,
    InProgress = 2,
    OnHold = 3,
    Completed = 4,
    Canceled = 5
}

/// <summary>
/// Represents a simple subtask structure to be stored within a TaskItem as JSON.
/// </summary>
public class SubTask
{
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; } = false;
}