using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces;

/// <summary>
/// Defines data persistence operations and cross-tenant boundary isolation methods for the Tag entity.
/// </summary>
public interface ITagRepository
{
    /// <summary>
    /// Retrieves all tags belonging to a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of <see cref="Tag"/> entities.</returns>
    Task<IEnumerable<Tag>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Retrieves a tag by its identifier, scoped to the owning user.
    /// </summary>
    /// <param name="id">The unique identifier of the tag.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The matching <see cref="Tag"/> if found; otherwise, <c>null</c>.</returns>
    Task<Tag?> GetByIdAndUserIdAsync(Guid id, string userId);

    /// <summary>
    /// Persists a new tag entity into the database.
    /// </summary>
    /// <param name="tag">The tag entity to insert.</param>
    /// <returns><c>true</c> if the operation succeeds; otherwise, <c>false</c>.</returns>
    Task<bool> AddAsync(Tag tag);

    /// <summary>
    /// Removes a tag record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the tag to delete.</param>
    /// <returns><c>true</c> if the operation succeeds; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteAsync(Guid id);
}