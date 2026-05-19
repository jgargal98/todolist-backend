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
    /// Presentation Layer: AuthController.
    /// Cleanest possible implementation. Errors are handled by the Middleware.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await authService.LoginAsync(request);

        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var response = await authService.GenerateAuthResponse(user);
        return Ok(response);
    }

    /// <summary>
    /// Presentation Layer: AuthController.
    /// Handles registration requests. No try-catch needed due to Middleware.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Attempt to register
            var user = await authService.RegisterAsync(request);

            // Handle the null case (Email already taken)
            if (user is null)
            {
                return Conflict(new { message = "User with this email already exists." });
            }

            // If successful, generate tokens
            var response = await authService.GenerateAuthResponse(user);
            return CreatedAtAction(nameof(Login), response);
        }
        catch (Exception ex)
        {
            // Catch the exception thrown by the service and return the error message
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Exchanges an expired access token for a new pair of tokens.
    /// </summary>
    /// <param name="request">The expired JWT and valid Refresh Token.</param>
    /// <returns>A new <see cref="AuthResponse"/> if the session is valid; otherwise, 401 Unauthorized.</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        // Attempt to refresh the session using the provided token
        var response = await authService.RefreshTokenAsync(request);

        if (response is null)
        {
            // Return 401 if the token is invalid, expired, or doesn't match any user
            return Unauthorized(new { message = "Invalid or expired refresh token." });
        }

        // Return the new pair of tokens (Access Token + New Refresh Token)
        return Ok(response);
    }
}