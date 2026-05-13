using System.Security.Claims;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

/// <summary>
/// Orchestrates authentication flows by interacting with domain abstractions.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    /// <summary>
    /// Authenticates a user via email and password.
    /// </summary>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !await _userRepository.CheckPasswordAsync(user, request.Password))
        {
            return null;
        }

        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Authenticates a user via an expired Access Token and a valid Refresh Token.
    /// </summary>
    public async Task<AuthResponse?> RefreshAsync(RefreshRequest request)
    {
        var principal = _jwtProvider.GetPrincipalFromExpiredToken(request.AccessToken);
        var email = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email)) return null;

        var user = await _userRepository.GetByEmailAsync(email);

        // Explicit check to satisfy the compiler and ensure domain safety
        if (user is null || !IsRefreshTokenValid(user, request.RefreshToken))
        {
            return null;
        }

        // Now 'user' is guaranteed to be non-null for GenerateAuthResponse
        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Validates if the provided refresh token matches the stored one and is not expired.
    /// </summary>
    private bool IsRefreshTokenValid(User? user, string providedToken)
    {
        return user is not null &&
               user.RefreshToken == providedToken &&
               user.RefreshTokenExpiryTime > DateTime.UtcNow;
    }

    /// <summary>
    /// Centralized method to update user state and generate the final DTO.
    /// </summary>
    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var accessToken = _jwtProvider.Generate(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(user);

        return new AuthResponse(accessToken, refreshToken, user.Email!);
    }
}