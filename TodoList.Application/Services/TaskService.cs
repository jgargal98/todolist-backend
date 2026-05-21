using AutoMapper;
using TodoList.Application.DTOs.Task;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

/// <summary>
/// Core sealed application service implementing transactional workflows and data orchestration for tasks.
/// </summary>
public sealed class TaskService(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    ITagRepository tagRepository,
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

        // 2. Map sanitized structural strings directly into a pristine Domain Entity instance
        var newTask = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DueDate = request.DueDate,
            Status = request.Status,
            UserId = userId,
            CategoryId = request.CategoryId,
            SubTasks = request.SubTasks.Select(st => new SubTask
            {
                Title = st.Title.Trim(),
                IsDone = st.IsDone
            }).ToList()
        };

        // 3. Coordinate optional many-to-many tag references mapping using the helper method
        var tagSyncSuccess = await SyncTaskTagsAsync(newTask, userId, request.TagIds);
        if (!tagSyncSuccess)
        {
            return null;
        }

        // 4. Call infrastructure storage repository layer to commit structural entities
        var registered = await taskRepository.AddAsync(newTask);
        if (!registered)
        {
            throw new Exception("Database error: Could not record task into SQL Server.");
        }

        // 5. Transform Domain Graph to simple DTO to fully break serialization loops
        return mapper.Map<TaskResponse>(newTask);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateTaskAsync(Guid taskId, string userId, UpdateTaskRequest request)
    {
        // 1. Retrieve the domain tracking entity directly using the eager-loading relational repository method
        var task = await taskRepository.GetByIdWithTagsAsync(taskId, userId);

        // Security Validation Boundary: Ensure the task exists and belongs to the current request context user
        if (task is null || task.UserId != userId)
        {
            return false;
        }

        // 2. Apply incoming mapped data directly onto the domain entity properties
        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.DueDate = request.DueDate;
        task.Status = request.Status;
        task.CategoryId = request.CategoryId;

        // 3. Subtasks mapping
        task.SubTasks = request.SubTasks.Select(subTaskDto => new SubTask
        {
            Title = subTaskDto.Title.Trim(),
            IsDone = subTaskDto.IsDone
        }).ToList();

        // 4. Coordinate optional many-to-many tag references mapping using the helper method
        var tagSyncSuccess = await SyncTaskTagsAsync(task, userId, request.TagIds);
        if (!tagSyncSuccess)
        {
            return false;
        }

        // 5. Persist modified entity state variables within the database context
        return await taskRepository.UpdateAsync(task);
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

        // 2. Locate targeted element within the isolated domain query response using LINQ Method Syntax
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

    /// <inheritdoc />
    public async Task<IEnumerable<TaskResponse>> GetTasksByUserIdAsync(string userId)
    {
        // 1. Fetch database domain entities using the infrastructure layer repository.
        var tasks = await taskRepository.GetByUserIdAsync(userId);

        // 2. Prevent null reference exceptions downstream by returning an empty collection placeholder if data is missing.
        if (tasks is null)
        {
            return Enumerable.Empty<TaskResponse>();
        }

        // 3. Transform the domain graph collection into plain, serializable response DTO structures.
        return mapper.Map<IEnumerable<TaskResponse>>(tasks);
    }

    /// <summary>
    /// Reusable data synchronization mechanism that securely parses, validates, and binds an optional collection of tags to a task entity context.
    /// </summary>
    /// <param name="task">The destination domain entity being tracked.</param>
    /// <param name="userId">The tenant context owner verifying data integrity constraints.</param>
    /// <param name="tagIds">The collection of target tag identifiers to be evaluated.</param>
    /// <returns><c>true</c> if the relationship graph evaluation passes or is skipped; otherwise, <c>false</c>.</returns>
    private async Task<bool> SyncTaskTagsAsync(TaskItem task, string userId, List<Guid> tagIds)
    {
        // Drop existing reference attachments in context tracking memory state
        task.Tags.Clear();

        // Short-circuit execution if incoming payload collection is unassigned or empty (Optional Requirement)
        if (tagIds is null || !tagIds.Any())
        {
            return true;
        }

        // Extract registered elements assigned specifically to the demanding owner
        var validTags = await tagRepository.GetTagsByIdsAsync(tagIds, userId);

        // Security boundary assessment: catch cross-tenant data manipulations or unknown identities
        if (validTags.Count() != tagIds.Count)
        {
            return false;
        }

        // Rebuild tracking map boundaries safely
        foreach (var tag in validTags)
        {
            task.Tags.Add(tag);
        }

        return true;
    }
}