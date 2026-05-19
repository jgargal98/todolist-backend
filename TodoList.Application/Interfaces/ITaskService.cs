using TodoList.Application.DTOs.Task;
using TodoList.Domain.Entities;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Defines business use cases and operations available for handling tasks.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Asynchronously creates a new task assigned to a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the authenticated user.</param>
    /// <param name="request">The data context used to build the new task.</param>
    /// <returns>A task that represents the asynchronous operation, containing the created <see cref="TaskItem"/>.</returns>
    Task<TaskResponse?> CreateTaskAsync(string userId, CreateTaskRequest request);
}