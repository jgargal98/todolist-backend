namespace TodoList.Application.DTOs.Auth;

/// <summary>
/// Immutable record representing a new user registration data payload.
/// </summary>
/// <param name="Email">The desired unique email identity for the account.</param>
/// <param name="Password">The raw password string meeting complexity rules.</param>
/// <param name="ConfirmPassword">The structural confirmation password matching string.</param>
public record RegisterRequest(string Email, string Password, string ConfirmPassword);