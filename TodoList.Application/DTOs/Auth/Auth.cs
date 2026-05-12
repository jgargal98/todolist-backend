namespace TodoList.Application.DTOs.Auth;

/// <summary>
/// Represents the login credentials provided by the user.
/// </summary>
/// <param name="Email">The user's registered email address.</param>
/// <param name="Password">The user's plain-text password.</param>
public record LoginRequest(string Email, string Password);

/// <summary>
/// Represents the authentication response containing both access and refresh tokens.
/// </summary>
/// <param name="AccessToken">The short-lived JWT for API authentication.</param>
/// <param name="RefreshToken">The long-lived token used to obtain a new access token.</param>
/// <param name="Email">The authenticated user's email address.</param>
public record AuthResponse(string AccessToken, string RefreshToken, string Email);

/// <summary>
/// Represents a request to refresh an expired access token.
/// </summary>
/// <param name="AccessToken">The expired or nearly expired JWT.</param>
/// <param name="RefreshToken">The valid refresh token stored on the client.</param>
public record RefreshRequest(string AccessToken, string RefreshToken);