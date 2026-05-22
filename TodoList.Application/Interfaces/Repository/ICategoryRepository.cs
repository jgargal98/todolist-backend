using TodoList.Domain.Entities;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Defines the infrastructure data access contract for managing Category domain entities.
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Retrieves all category entities associated with a specific user context.
    /// </summary>
    /// <param name="userId">The unique identifier of the target user owner.</param>
    /// <returns>A collection of user-scoped categories.</returns>
    Task<IEnumerable<Category>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Retrieves a single category entity verified by its identifier and owner identity.
    /// </summary>
    /// <param name="id">The unique identifier of the target category.</param>
    /// <param name="userId">The unique identifier of the user context checking ownership.</param>
    /// <returns>The category entity if matched; otherwise, null.</returns>
    Task<Category?> GetByIdAndUserIdAsync(Guid id, string userId);

    /// <summary>
    /// Appends and persists a new category entity instance into the storage context.
    /// </summary>
    /// <param name="category">The pristine domain entity to be recorded.</param>
    /// <returns>True if the state change was successfully committed; otherwise, false.</returns>
    Task<bool> AddAsync(Category category);

    /// <summary>
    /// Synchronizes changes made to an already tracked category state into persistent storage.
    /// </summary>
    /// <param name="category">The mutated domain entity instance.</param>
    /// <returns>True if the modifications were successfully committed; otherwise, false.</returns>
    Task<bool> UpdateAsync(Category category);

    /// <summary>
    /// Evicts and removes a target category instance from the tracking context ledger.
    /// </summary>
    /// <param name="category">The tracked domain entity to remove.</param>
    /// <returns>True if the removal operation was successfully committed; otherwise, false.</returns>
    Task<bool> DeleteAsync(Category category);
}