using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoList.Application.DTOs.Task;
using TodoList.Application.Interfaces;

namespace TodoList.API.Controllers;

/// <summary>
/// Presentation Layer API: Exposes HTTP endpoints to manage user task assets.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    /// <summary>
    /// Creates a new task for the authenticated user.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Authentication context identity is missing." });
            }

            var response = await taskService.CreateTaskAsync(userId, request);

            if (response is null)
            {
                return BadRequest(new { message = "Invalid data payload. Check constraints." });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves all tasks belonging to the authenticated user.
    /// </summary>
    /// <returns>A list of <see cref="TaskResponse"/> for the current user.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponse>), 200)]
    public async Task<IActionResult> GetTasksFromUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }

        var response = await taskService.GetTasksByUserIdAsync(userId);
        return Ok(response);
    }

    /// <summary>
    /// Updates an existing task after verifying resource existence and user ownership.
    /// </summary>
    /// <param name="id">The unique identifier of the task to update.</param>
    /// <param name="request">The updated task data.</param>
    /// <returns>204 No Content if successful; 404 if the task was not found or access is denied.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }

        var isUpdated = await taskService.UpdateTaskAsync(id, userId, request);

        if (!isUpdated)
        {
            return NotFound(new { message = "The requested task was not found, or you do not have permission to modify it." });
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a task belonging to the authenticated user.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }

        var success = await taskService.DeleteTaskAsync(id, userId);

        if (!success)
        {
            return NotFound(new { message = $"Task with ID {id} was not found or access is denied." });
        }

        return NoContent();
    }
}