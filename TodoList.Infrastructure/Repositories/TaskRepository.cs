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
        await context.Tasks.AddAsync(task);
        await context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(TaskItem task)
    {
        context.Tasks.Update(task);
        await context.SaveChangesAsync();
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
        return await context.Tasks
            .Include(t => t.SubTasks)
            .Include(t => t.Tags)
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await context.Tasks.FindAsync(id);
        if (task is null)
        {
            return false;
        }

        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
        return true;
    }
}