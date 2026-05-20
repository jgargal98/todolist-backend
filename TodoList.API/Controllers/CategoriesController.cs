using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Category;
using TodoList.Application.Interfaces;

namespace TodoList.API.Controllers;

/// <summary>
/// Exposes structured secure endpoints to handle user-scoped categories operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// Initializes a new instance of <see cref="CategoriesController"/>.
    /// </summary>
    /// <param name="categoryService">The orchestration application workflow contract implementation.</param>
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Obtains all categories mapped exclusively under the current active identity claim context.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryResponse>))]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }
        var categories = await _categoryService.GetUserCategoriesAsync(userId);
        return Ok(categories);
    }

    /// <summary>
    /// Records and instantiates a pristine custom category resource under the user space.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }
        var response = await _categoryService.CreateCategoryAsync(userId, request);

        if (response == null)
        {
            // Returns a Bad Request if user validation routines fail
            return BadRequest(new { message = "Invalid user identity context state execution data." });
        }

        return CreatedAtAction(nameof(GetAll), new { id = response.Id }, response);
    }

    /// <summary>
    /// Modifies structural properties of a localized user category asset item.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Authentication context identity is missing." });
        }
        var success = await _categoryService.UpdateCategoryAsync(id, userId, request);

        if (!success)
        {
            return NotFound(new { message = "The requested category resource does not exist or access is forbidden." });
        }

        return NoContent();
    }

    /// <summary>
    /// Purges a single category asset item from persistent storage engines.
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
        var success = await _categoryService.DeleteCategoryAsync(id, userId);

        if (!success)
        {
            return NotFound(new { message = "The requested category resource does not exist or access is forbidden." });
        }

        return NoContent();
    }
}