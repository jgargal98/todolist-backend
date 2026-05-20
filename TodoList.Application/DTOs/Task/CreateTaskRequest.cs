using System;
using System.Collections.Generic;

namespace TodoList.Application.DTOs.Task;

/// <summary>
/// Data Transfer Object for creating a task, including its initial status, category, and subtasks payload.
/// </summary>
public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }

    /// <summary>Defaults to Pending if not explicitly provided by the client.</summary>
    public int Status { get; set; } = 1;
    public Guid? CategoryId { get; set; }

    /// <summary>Optional collection of subtasks to be embedded as JSON.</summary>
    public List<CreateSubTaskRequest> SubTasks { get; set; } = new();
}

/// <summary>
/// Nested DTO representing the initial state of a subtask.
/// </summary>
public class CreateSubTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; } = false;
}