using System;

namespace TodoList.Application.DTOs.Task;

/// <summary>
/// Represents the data transfer object containing the allowed updatable fields for a task.
/// </summary>
/// <param name="Title">The updated title text for the task item.</param>
/// <param name="Description">The optional updated descriptive body text.</param>
/// <param name="DueDate">The optional updated target completion date and time.</param>
/// <param name="Status">The integer status code representing the state of the task.</param>
public record UpdateTaskRequest(string Title, string? Description, DateTime? DueDate, int Status);