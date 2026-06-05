namespace TodoList.Domain.Entities;

/// <summary>
/// Represents the possible states of a task according to the ERD.
/// </summary>
public enum TaskStatus
{
    /// <summary>Task has not been started yet.</summary>
    Pending = 1,
    /// <summary>Task is currently being worked on.</summary>
    InProgress = 2,
    /// <summary>Task is temporarily paused.</summary>
    OnHold = 3,
    /// <summary>Task has been finished.</summary>
    Completed = 4,
    /// <summary>Task was cancelled before completion.</summary>
    Canceled = 5
}

/// <summary>
/// Represents a simple subtask structure to be stored within a TaskItem as JSON.
/// </summary>
public class SubTask
{
    /// <summary>Title or description of the subtask.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Indicates whether the subtask has been completed.</summary>
    public bool IsDone { get; set; } = false;
}