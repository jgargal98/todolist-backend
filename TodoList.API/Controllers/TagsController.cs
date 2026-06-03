using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.Interfaces;

namespace TodoList.API.Controllers;

/// <summary>
/// Handles CRUD operations for user-scoped tags.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    /// <summary>
    /// Initializes a new instance of <see cref="TagsController"/>.
    /// </summary>
    /// <param name="tagService">The tag service dependency.</param>
    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    /// <summary>
    /// Retrieves all tags for the authenticated user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TagResponse>))]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }
        var tags = await _tagService.GetUserTagsAsync(userId);
        return Ok(tags);
    }

    /// <summary>
    /// Creates a new tag for the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TagResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }
        var response = await _tagService.CreateTagAsync(userId, request);

        if (response == null)
        {
            return BadRequest(new { message = "Invalid user identity context state execution data." });
        }

        return CreatedAtAction(nameof(GetAll), new { id = response.Id }, response);
    }

    /// <summary>
    /// Deletes a tag belonging to the authenticated user.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }
        var success = await _tagService.DeleteTagAsync(id, userId);

        if (!success)
        {
            return NotFound(new { message = "The requested tag resource does not exist or access is forbidden." });
        }

        return NoContent();
    }
}