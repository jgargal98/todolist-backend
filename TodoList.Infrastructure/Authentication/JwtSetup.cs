using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TodoList.Infrastructure.Authentication;

/// <summary>
/// Handles the technical configuration for JWT Bearer Authentication.
/// This class ensures the DependencyInjection file remains clean and focused.
/// </summary>
public class JwtSetup(IConfiguration configuration) : IConfigureNamedOptions<JwtBearerOptions>
{
    /// <summary>Delegates configuration to the named overload regardless of the scheme name.</summary>
    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }

    /// <summary>
    /// Configures the JWT validation parameters using settings from appsettings.json.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(JwtBearerOptions options)
    {
        // 1. Get the Public Key from configuration
        string publicKeyPem = configuration["Jwt:PublicKey"]
            ?? throw new InvalidOperationException("JWT PublicKey not found in configuration.");

        // 2. Create the RSA object and import the PEM key
        var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportFromPem(publicKeyPem);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],

            // 3. Use RsaSecurityKey instead of SymmetricSecurityKey
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    }
}