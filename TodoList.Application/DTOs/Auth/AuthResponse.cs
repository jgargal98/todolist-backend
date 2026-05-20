namespace TodoList.Application.DTOs.Auth;

/// <summary>
/// Represents the successful authentication response payload containing token bundles.
/// </summary>
/// <param name="AccessToken">The short-lived cryptographically signed JWT token.</param>
/// <param name="RefreshToken">The long-lived token utilized to safely rotate expired access keys.</param>
/// <param name="Email">The verified email context of the authenticated user.</param>
public record AuthResponse(string AccessToken, string RefreshToken, string Email);