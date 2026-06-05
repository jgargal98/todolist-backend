using TodoList.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Interfaces;

namespace TodoList.API.Controllers;

/// <summary>
/// Controller responsible for managing user identity and session tokens.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and returns the corresponding access and refresh tokens.
    /// </summary>
    /// <param name="request">The login request containing user credentials.</param>
    /// <returns>An <see cref="IActionResult"/> with the <see cref="AuthResponse"/> if successful; otherwise, 401 Unauthorized or 500 Internal Server Error.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await authService.LoginAsync(request);

            if (response is null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login.", details = ex.Message });
        }
    }

    /// <summary>
    /// Registers a new user and immediately generates their initial session tokens.
    /// </summary>
    /// <param name="request">The registration request containing email and password.</param>
    /// <returns>An <see cref="IActionResult"/> with the created <see cref="AuthResponse"/>; otherwise, 409 Conflict or 400 Bad Request.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await authService.RegisterAsync(request);

            if (response is null)
            {
                return Conflict(new { message = "User with this email already exists." });
            }

            return CreatedAtAction(nameof(Login), response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Exchanges an expired access token for a new pair of tokens using a valid refresh token.
    /// </summary>
    /// <param name="request">The refresh request containing the expired access token and valid refresh token.</param>
    /// <returns>An <see cref="IActionResult"/> with the new <see cref="AuthResponse"/>; otherwise, 401 Unauthorized or 400 Bad Request.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            var response = await authService.RefreshTokenAsync(request);

            if (response is null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error processing refresh token.", details = ex.Message });
        }
    }
}