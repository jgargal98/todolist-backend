namespace TodoList.Application.DTOs.Auth;

/// <summary>
/// Payload contract used to request a token rotation when an access key expires.
/// </summary>
/// <param name="AccessToken">The expired or nearly expired access token string.</param>
/// <param name="RefreshToken">The valid long-lived tracking rotation token.</param>
public record RefreshRequest(string AccessToken, string RefreshToken);