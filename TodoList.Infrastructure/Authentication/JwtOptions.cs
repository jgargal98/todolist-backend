namespace TodoList.Infrastructure.Authentication;

/// <summary>
/// Represents the JWT configuration settings from appsettings.json.
/// </summary>
public class JwtOptions
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
}