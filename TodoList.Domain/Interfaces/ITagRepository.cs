using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces;

/// <summary>
/// Defines data persistence operations and cross-tenant boundary isolation methods for the Tag entity.
/// </summary>
public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetByUserIdAsync(string userId);
    Task<Tag?> GetByIdAndUserIdAsync(Guid id, string userId);
    Task<bool> AddAsync(Tag tag);
    Task<bool> DeleteAsync(Guid id);
}