using System.Threading.Tasks;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure.Repositories;

/// <summary>
/// Infrastructure Layer: TaskRepository.
/// Implements data access operations for tasks using Entity Framework Core.
/// </summary>
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
}