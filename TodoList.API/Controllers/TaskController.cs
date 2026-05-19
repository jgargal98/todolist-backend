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
}