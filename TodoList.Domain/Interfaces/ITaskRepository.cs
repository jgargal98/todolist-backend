using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces;

/// <summary>
/// Defines data persistence contracts and database operations for TaskItem entities.
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Persists a new task aggregate root into the database tracking context.
    /// </summary>
    /// <param name="task">The task domain entity instance to insert.</param>
    /// <returns><c>true</c> if the database insert operation succeeds; otherwise, <c>false</c>.</returns>
    Task<bool> AddAsync(TaskItem task);

    /// <summary>
    /// Persists state modifications of a tracked task entity into the database.
    /// </summary>
    /// <param name="task">The modified task entity instance.</param>
    /// <returns><c>true</c> if the update succeeds; otherwise, <c>false</c>.</returns>
    Task<bool> UpdateAsync(TaskItem task);

    /// <summary>
    /// Retrieves a task entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the task.</param>
    /// <returns>The matching <see cref="TaskItem"/> if found; otherwise, <c>null</c>.</returns>
    Task<TaskItem?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all tasks assigned to a specific user, including related child subtasks.
    /// </summary>
    /// <param name="userId">The unique tracking identifier string of the resource owner.</param>
    /// <returns>A collection of matching <see cref="TaskItem"/> entities belonging to the user.</returns>
    Task<IEnumerable<TaskItem>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Removes a task record from the persistence context by its unique identifier.
    /// </summary>
    /// <param name="id">The explicit tracking GUID of the target task row.</param>
    /// <returns><c>true</c> if the entity row is successfully removed and saved; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteAsync(Guid id);
}