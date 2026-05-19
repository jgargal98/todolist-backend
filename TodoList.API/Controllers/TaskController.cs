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

            // Fire the application service layer containing our business constraints
            var task = await taskService.CreateTaskAsync(userId, request);

            // Catch business verification failures (invalid user or status out of range 1-5)
            if (task is null)
            {
                return BadRequest(new { message = "Invalid data payload. Ensure status is between 1 and 5 and user context exists." });
            }

            // Return 201 Created alongside metadata route telemetry
            return CreatedAtAction(nameof(Create), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            // Safely intercept infrastructure SQL Server drops or critical connection failures
            return BadRequest(new { message = ex.Message });
        }
    }
}