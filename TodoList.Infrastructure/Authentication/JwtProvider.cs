using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Infrastructure.Authentication;

/// <summary>
/// Provides methods to generate and validate JSON Web Tokens using RSA Asymmetric encryption.
/// Implements <see cref="ITokenProvider"/> for clean architecture abstraction.
/// </summary>
public sealed class JwtProvider : ITokenProvider
{
    private readonly JwtOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtProvider"/> class.
    /// </summary>
    /// <param name="options">The JWT settings injected via IOptions.</param>
    public JwtProvider(IOptions<JwtOptions> options) => _options = options.Value;

    /// <summary>
    /// Generates a JWT for a specific user signed with the RSA Private Key.
    /// </summary>
    /// <param name="user">The user entity for which the token is being generated.</param>
    /// <returns>A signed JWT string using the RS256 algorithm.</returns>
    public string Generate(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
        };

        // Initialize RSA with the Private Key for digital signing
        var rsa = RSA.Create();
        rsa.ImportFromPem(_options.PrivateKey);
        var key = new RsaSecurityKey(rsa);

        // Define credentials using RS256 (Asymmetric)
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a cryptographically strong random string to be used as a Refresh Token.
    /// </summary>
    /// <returns>A Base64 encoded string representing the refresh token.</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Validates an expired token and retrieves the <see cref="ClaimsPrincipal"/>.
    /// This is used during the Refresh Token flow.
    /// </summary>
    /// <param name="token">The expired or nearly expired JWT.</param>
    /// <returns>The principal if the token is valid (ignoring expiration); otherwise, null.</returns>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(_options.PublicKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa), // Verification uses Public Key
            ValidateLifetime = false // We specifically allow expired tokens here
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        // Ensure the token was signed with the expected RSA algorithm
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        return principal;
    }
}