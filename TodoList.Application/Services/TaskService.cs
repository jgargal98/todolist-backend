using AutoMapper;
using TodoList.Application.DTOs.Task;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;

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
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return null;
        }

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

        var tagSyncSuccess = await SyncTaskTagsAsync(newTask, userId, request.TagIds);
        if (!tagSyncSuccess)
        {
            return null;
        }

        var registered = await taskRepository.AddAsync(newTask);
        if (!registered)
        {
            throw new Exception("Database error: Could not record task into SQL Server.");
        }

        return mapper.Map<TaskResponse>(newTask);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateTaskAsync(Guid taskId, string userId, UpdateTaskRequest request)
    {
        var task = await taskRepository.GetByIdWithTagsAsync(taskId, userId);

        if (task is null || task.UserId != userId)
        {
            return false;
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.DueDate = request.DueDate;
        task.Status = request.Status;
        task.CategoryId = request.CategoryId;

        task.SubTasks = request.SubTasks.Select(subTaskDto => new SubTask
        {
            Title = subTaskDto.Title.Trim(),
            IsDone = subTaskDto.IsDone
        }).ToList();

        var tagSyncSuccess = await SyncTaskTagsAsync(task, userId, request.TagIds);
        if (!tagSyncSuccess)
        {
            return false;
        }

        return await taskRepository.UpdateAsync(task);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteTaskAsync(Guid taskId, string userId)
    {
        var userTasks = await taskRepository.GetByUserIdAsync(userId);
        if (userTasks is null)
        {
            return false;
        }

        var taskToDelete = userTasks.FirstOrDefault(t => t.Id == taskId);
        if (taskToDelete is null)
        {
            return false;
        }

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
        var tasks = await taskRepository.GetByUserIdAsync(userId);

        if (tasks is null)
        {
            return Enumerable.Empty<TaskResponse>();
        }

        return mapper.Map<IEnumerable<TaskResponse>>(tasks);
    }

    /// <summary>
    /// Synchronizes the many-to-many tag relationship for a task, ensuring all tag IDs
    /// belong to the requesting user and exist in the database.
    /// </summary>
    /// <param name="task">The task entity whose tags will be updated.</param>
    /// <param name="userId">The owner identifier for cross-tenant validation.</param>
    /// <param name="tagIds">The collection of tag identifiers to associate.</param>
    /// <returns><c>true</c> if the synchronization succeeds or is skipped; otherwise, <c>false</c>.</returns>
    private async Task<bool> SyncTaskTagsAsync(TaskItem task, string userId, List<Guid> tagIds)
    {
        task.Tags.Clear();

        if (tagIds is null || tagIds.Count == 0)
        {
            return true;
        }

        var validTags = await tagRepository.GetTagsByIdsAsync(tagIds, userId);

        var requestedTagIds = tagIds.Distinct().ToHashSet();
        var returnedTagIds = validTags.Select(t => t.Id).ToHashSet();
        if (!requestedTagIds.SetEquals(returnedTagIds))
        {
            return false;
        }

        foreach (var tag in validTags)
        {
            task.Tags.Add(tag);
        }

        return true;
    }
}