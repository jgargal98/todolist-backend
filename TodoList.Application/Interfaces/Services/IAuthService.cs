using TodoList.Application.DTOs.Auth;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Defines the application layer contract for managing authentication processes.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates credentials and generates session tokens for a user.
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);

    /// <summary>
    /// Creates a new user and generates their initial session tokens.
    /// </summary>
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Validates an expired access token and security keys to rotate session tokens.
    /// </summary>
    Task<AuthResponse?> RefreshTokenAsync(RefreshRequest request);
}