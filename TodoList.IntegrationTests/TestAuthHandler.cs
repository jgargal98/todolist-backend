using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TodoList.IntegrationTests;

/// <summary>
/// Replaces JWT Bearer authentication during integration tests.
/// By default it auto-authenticates every request with a fixed test identity.
/// Add the header "X-Bypass-Auth: true" to test 401 Unauthorized scenarios.
/// </summary>
public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string TestUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";
    public const string TestEmail = "testseed@todolist.com";
    public const string TestPassword = "TestSeed123!";
    public const string BypassAuthHeader = "X-Bypass-Auth";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.Request.Headers.ContainsKey(BypassAuthHeader))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authentication bypassed via test header."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, TestUserId),
            new Claim(ClaimTypes.Name, TestEmail),
            new Claim(ClaimTypes.Email, TestEmail),
            new Claim(JwtRegisteredClaimNames.Sub, TestUserId),
            new Claim(JwtRegisteredClaimNames.Email, TestEmail),
            new Claim(JwtRegisteredClaimNames.UniqueName, TestEmail)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
