using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Application.Interfaces;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure.Repositories;

/// <inheritdoc />
public class TaskRepository(AppDbContext context) : ITaskRepository
{
    /// <inheritdoc />
    public async Task<bool> AddAsync(TaskItem task)
    {
        // 1. Append the task entity to the context tracking state
        var result = await context.Tasks.AddAsync(task);

        // 2. Persist to SQL Server. If it fails or throws, your Controller's try-catch handles it.
        await context.SaveChangesAsync();

        // 3. Return true to signal that the infrastructure execution completed successfully
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(TaskItem task)
    {
        var result = context.Tasks.Update(task);
        // 2. Persist to SQL Server. If it fails or throws, your Controller's try-catch handles it.
        await context.SaveChangesAsync();

        // 3. Return true to signal that the infrastructure execution completed successfully
        return true;
    }

    /// <inheritdoc />
    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await context.Tasks.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<TaskItem?> GetByIdWithTagsAsync(Guid id, string userId)
    {
        return await context.Tasks
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(string userId)
    {
        // Eagerly load the SubTasks collection to prevent lazy-loading issues during processing
        return await context.Tasks
            .Include(t => t.SubTasks)
            .Include(t => t.Tags)
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id)
    {
        // Locate the task within the database context tracking memory or storage
        var task = await context.Tasks.FindAsync(id);
        if (task is null)
        {
            return false;
        }

        // Remove the entry. Relational cascade paths will clean up associated SubTasks
        context.Tasks.Remove(task);

        // 2. Persist to SQL Server. If it fails or throws, your Controller's try-catch handles it.
        await context.SaveChangesAsync();

        // 3. Return true to signal that the infrastructure execution completed successfully
        return true;
    }
}