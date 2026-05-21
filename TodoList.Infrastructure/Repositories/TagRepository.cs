using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure.Repositories;

/// <summary>
/// Infrastructure Layer: TagRepository.
/// Implements data access operations for tags using Entity Framework Core.
/// </summary>
public class TagRepository(AppDbContext context) : ITagRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Tag>> GetByUserIdAsync(string userId)
    {
        return await context.Tags
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Tag?> GetByIdAndUserIdAsync(Guid id, string userId)
    {
        return await context.Tags
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    /// <inheritdoc />
    public async Task<bool> AddAsync(Tag tag)
    {
        // 1. Append the tag entity to the context tracking state
        await context.Tags.AddAsync(tag);

        // 2. Persist to SQL Server. If it fails or throws, your Controller's try-catch handles it.
        await context.SaveChangesAsync();

        // 3. Return true to signal that the infrastructure execution completed successfully
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id)
    {
        // 1. Locate the tag within the database context tracking memory or storage
        var tag = await context.Tags.FindAsync(id);
        if (tag is null)
        {
            return false;
        }

        // 2. Remove the entry from the database context
        context.Tags.Remove(tag);

        // 3. Persist to SQL Server. If it fails or throws, your Controller's try-catch handles it.
        await context.SaveChangesAsync();

        // 4. Return true to signal that the infrastructure execution completed successfully
        return true;
    }
}