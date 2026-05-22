using Microsoft.EntityFrameworkCore;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core concrete implementation of the decoupled database operations contract.
/// </summary>
public class CategoryRepository(AppDbContext context) : ICategoryRepository
{

    /// <inheritdoc />
    public async Task<IEnumerable<Category>> GetByUserIdAsync(string userId)
    {
        // Querying records using strict multi-tenant filtering conventions
        return await context.Categories
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Category?> GetByIdAndUserIdAsync(Guid id, string userId)
    {
        // Enforcing security containment parameters at database access level execution
        return await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    /// <inheritdoc />
    public async Task<bool> AddAsync(Category category)
    {
        try
        {
            await context.Categories.AddAsync(category);
            // Directly committing changes avoiding primitive state comparison shortcuts
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            // Bubble down state failures allowing higher application layers to coordinate errors safely
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Category category)
    {
        try
        {
            context.Categories.Update(category);
            // Synchronize state snapshot modifications to SQL Server database architecture
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Category category)
    {
        try
        {
            context.Categories.Remove(category);
            // Execute physical deletion routine tracking references
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}