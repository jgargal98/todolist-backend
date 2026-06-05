using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Interfaces;
using TodoList.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;

namespace TodoList.API.Controllers;

/// <summary>Controller for managing user-related operations (admin).</summary>
[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>Initializes a new instance of the <see cref="UsersController"/> class.</summary>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves all registered users.
    /// </summary>
    /// <returns>A list of <see cref="UserResponseDto"/> representing all users.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }
}