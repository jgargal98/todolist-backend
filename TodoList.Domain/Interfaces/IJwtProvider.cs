using System.Security.Claims;
using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces;

/// <summary>
/// Defines the contract for JWT operations and token validation.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Generates a signed JSON Web Token for the specified user.
    /// </summary>
    string Generate(User user);

    /// <summary>
    /// Creates a cryptographically secure random string for refresh purposes.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates an expired token and extracts its claims principal.
    /// </summary>
    /// <param name="token">The expired JWT string.</param>
    /// <returns>The <see cref="ClaimsPrincipal"/> if valid; otherwise, null.</returns>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}