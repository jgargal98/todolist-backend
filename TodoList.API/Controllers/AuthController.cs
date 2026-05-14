using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs.Auth;
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
        // If this fails, the execution "jumps" directly to the Middleware.
        var response = await authService.LoginAsync(request);

        // If it succeeds, we just return the data.
        return Ok(response);
    }

    /// <summary>
    /// Presentation Layer: AuthController.
    /// Handles registration requests. No try-catch needed due to Middleware.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // The framework validates the DTO format before this code runs.
        // Any business/Identity error during execution will trigger the Middleware.
        var response = await authService.RegisterAsync(request);

        // Return 201 Created with the authentication tokens.
        return Created(string.Empty, response);
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

        return Ok(response);
    }
}