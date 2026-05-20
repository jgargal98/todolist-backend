namespace TodoList.Application.DTOs.Auth;

/// <summary>
/// Immutable record representing the login credentials provided by a client.
/// </summary>
/// <param name="Email">The user's registered email address target.</param>
/// <param name="Password">The user's plain-text password payload.</param>
public record LoginRequest(string Email, string Password);