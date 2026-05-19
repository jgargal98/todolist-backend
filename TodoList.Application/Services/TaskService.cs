using TodoList.Application.DTOs.Task;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

public sealed class TaskService(ITaskRepository taskRepository, IUserRepository userRepository) : ITaskService
{
    public async Task<TaskItem?> CreateTaskAsync(string userId, CreateTaskRequest request)
    {
        // 1. Check if the user exists in SQL Server
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return null;
        }

        // 2. Validate Text Constraints (No empty titles, max 100 characters)
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 100)
        {
            return null;
        }

        // 3. Validate Numeric Status Range (Strictly 1 to 5)
        if (request.Status < 1 || request.Status > 5)
        {
            return null;
        }

        // 4. Validate Future Date Constraint
        // If a DueDate is provided, it cannot be in the past
        if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow)
        {
            return null;
        }

        // 5. Validate SubTasks Integrity
        // Reject the request if any incoming subtask has an empty title
        if (request.SubTasks.Any(st => string.IsNullOrWhiteSpace(st.Title)))
        {
            return null;
        }

        // 6. Map data directly into the Domain Entity using plain INT
        var newTask = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(), // Clean accidental spaces
            Description = request.Description?.Trim(),
            DueDate = request.DueDate,
            Status = request.Status,
            UserId = userId,
            SubTasks = request.SubTasks.Select(st => new SubTask
            {
                Title = st.Title.Trim(),
                IsDone = st.IsCompleted
            }).ToList()
        };

        // 7. Persist into the database
        var registered = await taskRepository.AddAsync(newTask);

        if (!registered)
        {
            throw new Exception("Database error: Could not record task into SQL Server.");
        }

        return newTask;
    }
}