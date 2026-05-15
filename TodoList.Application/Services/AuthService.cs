using System.Security.Claims;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

/// <summary>
/// Orchestrates authentication flows by interacting with domain abstractions.
/// </summary>
public sealed class AuthService(IUserRepository userRepository, ITokenProvider jwtProvider) : IAuthService
{
    /// <summary>
    /// Application Layer: AuthService.
    /// Orchestrates the login process.
    /// </summary>
    public async Task<User?> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.ValidateCredentialsAsync(request.Email, request.Password);
        return user;
    }

    /// <summary>
    /// Application Layer: AuthService.
    /// Coordinates user registration and initial token generation.
    /// </summary>
    public async Task<User?> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await userRepository.ValidateCredentialsAsync(request.Email, request.Password);

        if (existingUser != null) return null;

        var newUser = new User { Email = request.Email, UserName = request.Email };

        var registered = await userRepository.CreateAsync(newUser, request.Password);

        if (!registered)
        {
            throw new Exception("Database error: Could not create user.");
        }

        return newUser;
    }

    /// <summary>
    /// Validates an expired access token and a valid refresh token to issue new ones.
    /// </summary>
    public async Task<AuthResponse?> RefreshTokenAsync(RefreshRequest request)
    {
        // 1. Extract the user identity from the expired access token.
        var principal = jwtProvider.GetPrincipalFromExpiredToken(request.AccessToken);
        var email = principal?.Identity?.Name;

        // Fix CS8604: Ensure email is not null before proceeding
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        // 2. Retrieve the user from the database using the identity found in the token.
        var user = await userRepository.GetByEmailAsync(email);

        // 3. Security Validation:
        // - Ensure the user exists
        // - Verify that the provided Refresh Token matches the one stored in the database.
        // - Check if the Refresh Token is still within its validity period (not expired).
        if (user is null || user.RefreshToken != request.AccessToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null; // Fail silently to prevent leaking which specific check failed.
        }

        // 4. Token Rotation:
        // Generate a brand-new Access Token and a new Refresh Token.
        // This invalidates the old refresh token to mitigate the risk of token theft.
        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Updates user session state and creates the AuthResponse DTO.
    /// </summary>
    public async Task<AuthResponse?> GenerateAuthResponse(User user)
    {
        var accessToken = jwtProvider.Generate(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        var updated = await userRepository.UpdateAsync(user);
        if (!updated) return null;

        return new AuthResponse(accessToken, refreshToken, user.Email!);
    }
}