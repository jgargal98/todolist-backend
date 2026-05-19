using TodoList.Application.DTOs.Auth;
using TodoList.Domain.Entities;


namespace TodoList.Application.Interfaces;

/// <summary>
/// Defines the contract for authentication and authorization services.
/// </summary>
/// <remarks>
/// This service handles the business logic for user identity verification 
/// and the management of security tokens.
/// </remarks>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user based on provided credentials.
    /// </summary>
    /// <param name="request">The login request containing email and password.</param>
    /// <returns>
    /// A <see cref="AuthResponse"/> containing the JWT and refresh token if successful; 
    /// otherwise, <see langword="null"/>.
    /// </returns>
    Task<User?> LoginAsync(LoginRequest request);

    /// <summary>
    /// Register a new user
    /// </summary>
    Task<User?> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Generates a new access token using a valid refresh token.
    /// </summary>
    /// <param name="request">The request containing the expired access token and valid refresh token.</param>
    /// <returns>
    /// A new <see cref="AuthResponse"/> with updated tokens if the refresh token is valid; 
    /// otherwise, <see langword="null"/>.
    /// </returns>
    Task<AuthResponse?> RefreshTokenAsync(RefreshRequest request);

    /// <summary>
    /// Generates a new access token.
    /// </summary>
    Task<AuthResponse?> GenerateAuthResponse(User user);
}