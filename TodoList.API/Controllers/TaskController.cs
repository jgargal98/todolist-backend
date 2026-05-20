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
    /// Processes task initialization payload requests securely.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        try
        {
            // Gather the unique database id from the authenticated JWT claims metadata
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Authentication context identity is missing." });
            }

            // The service now returns a sanitized TaskResponse DTO, preventing object cycles
            var response = await taskService.CreateTaskAsync(userId, request);

            if (response is null)
            {
                return BadRequest(new { message = "Invalid data payload. Check constraints." });
            }

            // Return a standard 200 OK
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Safely intercept infrastructure SQL Server drops or critical connection failures
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// HTTP GET endpoint to retrieve the complete list of tasks belonging to the calling user context.
    /// </summary>
    /// <returns>
    /// A HTTP 200 OK status containing the array collection of <see cref="TaskResponse"/> data payloads.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponse>), 200)]
    public async Task<IActionResult> GetTasksFromUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }

        // Query the underlying domain data layers through the boundary service orchestration
        var response = await taskService.GetTasksByUserIdAsync(userId);

        // Return a standard successful operational payload. Missing records will correctly display an empty JSON array ([])
        return Ok(response);
    }

    /// <summary>
    /// Updates an existing task after verifying resource existence and user ownership.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the task to update.</param>
    /// <param name="request">The updated task data payload.</param>
    /// <returns>
    /// 204 No Content if successful; 
    /// 401 Unauthorized if the user claim is missing; 
    /// 404 Not Found if the task does not exist or belongs to another user.
    /// </returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        // Extract the unique identifier of the authenticated user from JWT Claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }

        // Process the update through the application service layer
        var isUpdated = await taskService.UpdateTaskAsync(id, userId, request);

        if (!isUpdated)
        {
            // Return 404 to prevent resource enumeration leaks regarding tasks owned by other users
            return NotFound(new { message = "The requested task was not found, or you do not have permission to modify it." });
        }

        return NoContent();
    }

    /// <summary>
    /// HTTP DELETE endpoint to remove an existing task.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        // Retrieve the authenticated user ID extracted from JWT claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }

        // Request the service layer to validate ownership and delete the resource
        var success = await taskService.DeleteTaskAsync(id, userId);

        if (!success)
        {
            // Return 404 Not Found if the task doesn't exist or doesn't belong to the user
            return NotFound(new { message = $"Task with ID {id} was not found or access is denied." });
        }

        // Return 204 No Content to indicate successful processing without a payload
        return NoContent();
    }
}