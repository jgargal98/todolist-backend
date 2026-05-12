using TodoList.Application.DTOs.Auth;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Application.Services;

/// <summary>
/// Orchestrates authentication flows by interacting with domain abstractions.
/// </summary>
public sealed class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    /// <summary>
    /// Validates credentials and manages session persistence through abstractions.
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
    /// Internal helper to set token state and delegate persistence to the repository.
    /// </summary>
    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var accessToken = _jwtProvider.Generate(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        // The Service updates the state of the Domain Entity
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        // The Service commands the Repository to persist the changes
        await _userRepository.UpdateAsync(user);

        return new AuthResponse(accessToken, refreshToken, user.Email!);
    }
}