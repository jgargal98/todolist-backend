using AutoMapper;
using TodoList.Application.DTOs.Task;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

/// <summary>
/// Core sealed application service implementing business validation and transactional workflows for tasks.
/// </summary>
public sealed class TaskService(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    IMapper mapper) : ITaskService
{
    /// <inheritdoc />
    public async Task<TaskResponse?> CreateTaskAsync(string userId, CreateTaskRequest request)
    {
        // 1. Verify target user record integrity exists inside the database context
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return null;
        }

        // 2. Validate Text Constraints: Non-empty titles bound to 100 character thresholds
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 100)
        {
            return null;
        }

        // 3. Validate Status State Fields: Strictly bounded between codes 1 and 5
        if (request.Status < 1 || request.Status > 5)
        {
            return null;
        }

        // 4. Validate Temporal Targets: Restrict due dates from representing past points in time
        if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow)
        {
            return null;
        }

        // 5. Validate Child Collections: Reject execution if any subtask title is invalid
        if (request.SubTasks.Any(st => string.IsNullOrWhiteSpace(st.Title)))
        {
            return null;
        }

        // 6. Map sanitized structural strings directly into a pristine Domain Entity instance
        var newTask = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DueDate = request.DueDate,
            Status = request.Status,
            UserId = userId,
            SubTasks = request.SubTasks.Select(st => new SubTask
            {
                Title = st.Title.Trim(),
                IsDone = st.IsDone
            }).ToList()
        };

        // 7. Call infrastructure storage repository layer to commit structural entities
        var registered = await taskRepository.AddAsync(newTask);

        if (!registered)
        {
            throw new Exception("Database error: Could not record task into SQL Server.");
        }

        // 8. Transform Domain Graph to simple DTO to fully break serialization loops
        return mapper.Map<TaskResponse>(newTask);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteTaskAsync(Guid taskId, string userId)
    {
        // 1. Fetch only collection groups mapped to the requesting owner ID
        var userTasks = await taskRepository.GetByUserIdAsync(userId);
        if (userTasks is null)
        {
            return false;
        }

        // 2. Locate targeted element within the isolated domain query response
        var taskToDelete = userTasks.FirstOrDefault(t => t.Id == taskId);
        if (taskToDelete is null)
        {
            return false;
        }

        // 3. Execute hard deletion row removal routine from the infrastructure framework
        var isDeleted = await taskRepository.DeleteAsync(taskToDelete.Id);
        if (!isDeleted)
        {
            throw new Exception("Database error: Could not complete the deletion operation in SQL Server.");
        }

        return true;
    }
}