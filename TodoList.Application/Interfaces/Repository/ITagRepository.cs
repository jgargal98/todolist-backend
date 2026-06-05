using TodoList.Domain.Entities;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Domain Layer: ITagRepository.
/// Defines data persistence operations and cross-tenant boundary isolation methods for the Tag entity.
/// </summary>
public interface ITagRepository
{
    /// <summary>
    /// Fetches all tag entities belonging strictly to a specific user context.
    /// </summary>
    /// <param name="userId">The unique identifier of the requesting user tenant.</param>
    /// <returns>A collection of <see cref="Tag"/> entities owned by the user.</returns>
    Task<IEnumerable<Tag>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Fetches a specific tag entity by its identifier, ensuring it belongs to the designated user tenant.
    /// </summary>
    /// <param name="id">The unique identifier of the target tag.</param>
    /// <param name="userId">The unique identifier of the requesting user tenant.</param>
    /// <returns>The matching <see cref="Tag"/> if found and authorized; otherwise, <c>null</c>.</returns>
    Task<Tag?> GetByIdAndUserIdAsync(Guid id, string userId);

    /// <summary>
    /// Fetches a verified array of tag entities matching a collection of identifiers, isolated by user ownership.
    /// </summary>
    /// <param name="tagIds">The collection of unique tag identifiers to query.</param>
    /// <param name="userId">The owner identifier verifying data security boundaries.</param>
    /// <returns>A collection of matching <see cref="Tag"/> entities belonging to the user context.</returns>
    Task<IEnumerable<Tag>> GetTagsByIdsAsync(List<Guid> tagIds, string userId);

    /// <summary>
    /// Appends a pristine tag entity instance to the tracking state for future persistence.
    /// </summary>
    /// <param name="tag">The new tag entity instance to be registered.</param>
    /// <returns><c>true</c> if the infrastructure data operation completes successfully; otherwise, <c>false</c>.</returns>
    Task<bool> AddAsync(Tag tag);

    /// <summary>
    /// Executes a hard deletion row removal routine for a specific tag entry.
    /// </summary>
    /// <param name="id">The unique identifier of the tag to delete.</param>
    /// <returns><c>true</c> if the entity row is successfully removed; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteAsync(Guid id);
}