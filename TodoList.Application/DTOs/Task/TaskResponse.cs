using TodoList.Application.DTOs.Tag;

namespace TodoList.Application.DTOs.Task;

/// <summary>Data transfer object representing a task for API responses.</summary>
public class TaskResponse
{
    /// <summary>Unique identifier of the task.</summary>
    public Guid Id { get; set; }
    /// <summary>Title of the task.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Optional detailed description.</summary>
    public string? Description { get; set; }
    /// <summary>Optional due date for the task.</summary>
    public DateTime? DueDate { get; set; }
    /// <summary>Current status code (1-5).</summary>
    public int Status { get; set; }
    /// <summary>Foreign key to an optional category.</summary>
    public Guid? CategoryId { get; set; }
    /// <summary>Collection of subtasks embedded in the response.</summary>
    public List<SubTaskResponse> SubTasks { get; set; } = new();
    public List<TagResponse> Tags { get; set; } = new();
}

/// <summary>Data transfer object representing a subtask within a task response.</summary>
public class SubTaskResponse
{
    /// <summary>Title of the subtask.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Indicates whether the subtask is completed.</summary>
    public bool IsDone { get; set; }
}