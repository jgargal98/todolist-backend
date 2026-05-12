using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.Services;

namespace TodoList.API.Controllers;

/// <summary>
/// Controller responsible for managing user identity and session tokens.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    /// <summary>
    /// Handles the initial user authentication process.
    /// </summary>
    /// <param name="request">The credentials provided by the user.</param>
    /// <returns>An <see cref="AuthResponse"/> if valid; otherwise, 401 Unauthorized.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await authService.LoginAsync(request);

        if (response is null)
        {
            return Unauthorized("Invalid credentials.");
        }

        return Ok(response);
    }

    /// <summary>
    /// Exchanges an expired access token for a new pair of tokens.
    /// </summary>
    /// <param name="request">The expired JWT and valid Refresh Token.</param>
    /// <returns>A new <see cref="AuthResponse"/> if the session is valid; otherwise, 401 Unauthorized.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        // Now that AuthService has RefreshAsync implemented, the controller just delegates
        var response = await authService.RefreshAsync(request);

        if (response is null)
        {
            return Unauthorized("The session has expired or the token is invalid.");
        }

        return Ok(response);
    }
}