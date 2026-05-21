using TodoList.Application.DTOs.Auth;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

/// <summary>
/// Orchestrates authentication flows by interacting with domain abstractions and token providers.
/// </summary>
public sealed class AuthService(IUserRepository userRepository, ITokenProvider jwtProvider) : IAuthService
{
    /// <summary>
    /// Validates user credentials and produces an authentication response if successful.
    /// </summary>
    /// <param name="request">The login request details.</param>
    /// <returns>An <see cref="AuthResponse"/> containing the generated tokens; otherwise, null.</returns>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.ValidateCredentialsAsync(request.Email, request.Password);

        if (user is null)
        {
            return null;
        }

        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Registers a unique user and produces an initial authentication response.
    /// </summary>
    /// <param name="request">The user registration data.</param>
    /// <returns>An <see cref="AuthResponse"/> containing the initial tokens; otherwise, null if the email is taken.</returns>
    /// <exception cref="Exception">Thrown when the data repository fails to insert the user record.</exception>
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return null;
        }

        var newUser = new User
        {
            Email = request.Email,
            UserName = request.Email
        };

        var registered = await userRepository.CreateAsync(newUser, request.Password);
        if (!registered)
        {
            throw new Exception("Database error: Could not create user.");
        }

        return await GenerateAuthResponse(newUser);
    }

    /// <summary>
    /// Validates the current token state and rotates the session credentials.
    /// </summary>
    /// <param name="request">The token refresh payload.</param>
    /// <returns>A new <see cref="AuthResponse"/> with rotated tokens; otherwise, null.</returns>
    public async Task<AuthResponse?> RefreshTokenAsync(RefreshRequest request)
    {
        var principal = jwtProvider.GetPrincipalFromExpiredToken(request.AccessToken);
        var email = principal?.Identity?.Name;

        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        var user = await userRepository.GetByEmailAsync(email);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Private utility method to centralize database token updates and DTO creation.
    /// </summary>
    private async Task<AuthResponse?> GenerateAuthResponse(User user)
    {
        var accessToken = jwtProvider.Generate(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        var updated = await userRepository.UpdateAsync(user);
        if (!updated)
        {
            return null;
        }

        return new AuthResponse(accessToken, refreshToken, user.Email!);
    }
}