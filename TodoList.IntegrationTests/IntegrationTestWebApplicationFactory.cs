using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace TodoList.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory that replaces EF Core's SQL Server with an InMemory database,
/// injects a test authentication handler that bypasses JWT, generates ephemeral RSA keys
/// so that the real JwtProvider can create tokens during auth tests, and seeds a test user.
/// </summary>
public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        // Inject dynamically-generated RSA keys so the real JwtProvider keeps working.
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            using var rsa = RSA.Create(2048);
            var privateKey = rsa.ExportRSAPrivateKeyPem();
            var publicKey = rsa.ExportRSAPublicKeyPem();

            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:PrivateKey"] = privateKey,
                ["Jwt:PublicKey"] = publicKey,
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            });
        });

        builder.ConfigureServices(services =>
        {
            // --- 1. Remove ALL EF Core internal DI registrations ---
            // This eliminates the SqlServer provider and any conflicting provider
            // services. The subsequent UseInMemoryDatabase call will re-register
            // only the InMemory provider's services.
            var efCorePrefix = "Microsoft.EntityFrameworkCore";
            var efDescriptors = services
                .Where(d =>
                    (d.ServiceType.Assembly.GetName().Name?.StartsWith(efCorePrefix) == true) ||
                    (d.ImplementationType?.Assembly.GetName().Name?.StartsWith(efCorePrefix) == true))
                .ToList();
            foreach (var descriptor in efDescriptors)
                services.Remove(descriptor);

            // Also remove DbContextOptions registrations (factory-based).
            var optionsDescs = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                            || d.ServiceType == typeof(DbContextOptions))
                .ToList();
            foreach (var descriptor in optionsDescs)
                services.Remove(descriptor);

            // --- 2. Register InMemory database (unique name per factory instance) ---
            var dbName = $"TodoListTestDb_{Guid.NewGuid()}";
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            // --- 3. Replace JWT Bearer handler with TestAuthHandler ---
            // The AddJwtBearer() call from Program.cs already registered "Bearer" as a scheme.
            // We can't call AddScheme("Bearer", ...) again (it would throw).
            // Instead, we register TestAuthHandler as a service and then
            // modify the existing Bearer scheme's handler type via PostConfigure.
            services.TryAddTransient<TestAuthHandler>();
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                if (options.Schemes.FirstOrDefault(s => s.Name == "Bearer") is { } bearer)
                {
                    bearer.HandlerType = typeof(TestAuthHandler);
                }
            });

            // --- 4. Seed a known test user so FK constraints are satisfied ---
            var sp = services.BuildServiceProvider(validateScopes: false);
            using var scope = sp.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            using var context = scopedProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();

            var userManager = scopedProvider.GetRequiredService<UserManager<User>>();
            SeedTestUserAsync(userManager).GetAwaiter().GetResult();
        });
    }

    private static async Task SeedTestUserAsync(UserManager<User> userManager)
    {
        var existing = await userManager.FindByEmailAsync(TestAuthHandler.TestEmail);
        if (existing is not null)
            return;

        var user = new User
        {
            Id = TestAuthHandler.TestUserId,
            UserName = TestAuthHandler.TestEmail,
            Email = TestAuthHandler.TestEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, TestAuthHandler.TestPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Test user seed failed: {errors}");
        }
    }

    /// <summary>
    /// Creates an HttpClient that will be auto-authenticated by <see cref="TestAuthHandler"/>.
    /// Protected endpoints receive a valid identity without needing a real JWT.
    /// </summary>
    public HttpClient CreateAuthenticatedClient()
    {
        return CreateClient();
    }

    /// <summary>
    /// Creates an HttpClient with the "X-Bypass-Auth" header.
    /// <see cref="TestAuthHandler"/> will reject the request, resulting in 401 Unauthorized.
    /// </summary>
    public HttpClient CreateUnauthenticatedClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.BypassAuthHeader, "true");
        return client;
    }
}
