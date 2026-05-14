using System.Text;
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
    public void Configure(string? name, JwtBearerOptions options)
    {
        // It simply redirects the call to the actual logic
        // regardless of the name ("Bearer", "Custom", etc.)
        Configure(options);
    }
    /// <summary>
    /// Configures the JWT validation parameters using settings from appsettings.json.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"] ??
                throw new InvalidOperationException("Jwt SecretKey not found.")))
        };
    }
}