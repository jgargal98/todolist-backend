using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Application.Interfaces;
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
    public async Task<IEnumerable<Tag>> GetTagsByIdsAsync(List<Guid> tagIds, string userId)
    {
        return await context.Tags
            .Where(t => tagIds.Contains(t.Id) && t.UserId == userId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> AddAsync(Tag tag)
    {
        await context.Tags.AddAsync(tag);
        await context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id)
    {
        var tag = await context.Tags.FindAsync(id);
        if (tag is null)
        {
            return false;
        }

        context.Tags.Remove(tag);
        await context.SaveChangesAsync();
        return true;
    }
}