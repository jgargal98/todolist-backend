using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Interfaces;
using TodoList.Application.DTOs.User;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// GET: api/users
    /// Returns a list of all registered users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }
}