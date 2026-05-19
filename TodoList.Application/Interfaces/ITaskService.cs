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

    /// <summary>
    /// Deletes a specific task ensuring it belongs to the authenticated user.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="userId">The unique identifier of the user requesting deletion.</param>
    /// <returns>True if the task was successfully deleted; otherwise, false.</returns>
    Task<bool> DeleteTaskAsync(Guid taskId, string userId);

    /// <summary>
    /// Retrieves all tasks assigned to a specific user and maps them into safe response DTOs.
    /// </summary>
    /// <param name="userId">The unique identifier string of the authenticated resource owner.</param>
    /// <returns>A collection of mapped <see cref="TaskResponse"/> objects matching the user identity.</returns>
    Task<IEnumerable<TaskResponse>> GetTasksByUserIdAsync(string userId);
}