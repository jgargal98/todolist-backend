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
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // If this method throws an exception, the execution stops here 
        // and bubbles up to the Controller's catch block.
        var user = await userRepository.ValidateCredentialsAsync(request.Email, request.Password);

        // If we reach this point, credentials are valid.
        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Application Layer: AuthService.
    /// Coordinates user registration and initial token generation.
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName
        };

        // If Identity rules fail, this call throws an exception and stops the flow.
        await userRepository.CreateAsync(user, request.Password);

        // If we reach this point, registration was successful.
        // We generate and return the tokens.
        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Validates an expired access token and a valid refresh token to issue new ones.
    /// </summary>
    public async Task<AuthResponse?> RefreshAsync(RefreshRequest request)
    {
        var principal = jwtProvider.GetPrincipalFromExpiredToken(request.AccessToken);
        var email = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            throw new Exception("Invalid token claims.");

        var user = await userRepository.GetByEmailAsync(email);

        // Validation logic integrated here to avoid redundant private methods
        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new Exception("Session expired or invalid refresh token.");
        }

        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Updates user session state and creates the AuthResponse DTO.
    /// </summary>
    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var accessToken = jwtProvider.Generate(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await userRepository.UpdateAsync(user);

        return new AuthResponse(accessToken, refreshToken, user.Email!);
    }
}