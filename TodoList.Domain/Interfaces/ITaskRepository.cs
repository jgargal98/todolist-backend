using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces;

/// <summary>
/// Defines the data persistence contract and data access operations for TaskItem entities.
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Asynchronously inserts a new task entity into the underlying data store.
    /// </summary>
    /// <param name="task">The task domain entity instance to persist.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task<bool> AddAsync(TaskItem task);
}