namespace TodoList.Infrastructure.Authentication;

/// <summary>
/// Represents the JWT configuration settings from appsettings.json.
/// </summary>
public class JwtOptions
{
    /// <summary>RSA private key in PEM format used for token signing.</summary>
    public string PrivateKey { get; set; } = string.Empty;
    /// <summary>RSA public key in PEM format used for token validation.</summary>
    public string PublicKey { get; set; } = string.Empty;
    /// <summary>Token issuer identifier.</summary>
    public string Issuer { get; set; } = string.Empty;
    /// <summary>Token audience identifier.</summary>
    public string Audience { get; set; } = string.Empty;
}