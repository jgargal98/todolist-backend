namespace TodoList.Infrastructure.Authentication;

/// <summary>
/// Represents the JWT configuration settings from appsettings.json.
/// </summary>
public class JwtOptions
{
    public string PrivateKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}