using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure;

/// <summary>
/// Provides utility methods to handle database migrations and initial data seeding.
/// This class ensures the database schema is up-to-date and contains essential records.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Synchronizes the database schema and performs data seeding.
    /// </summary>
    /// <param name="serviceProvider">The root service provider to resolve dependencies.</param>
    /// <param name="isDevelopment">A flag indicating if the environment is Development.</param>
    /// <exception cref="Exception">Throws an exception if the migration or seeding process fails.</exception>
    public static void InitializeDatabase(this IServiceProvider serviceProvider, bool isDevelopment)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            context.Database.SetCommandTimeout(120);

            // LOG DE CONTROL
            logger.LogInformation("Verifying sql tables...");

            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            logger.LogInformation("SQL TABLES SUCCESSFULLY VERIFIED.");

            // Intentamos el Seed
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            SeedAdminUserAsync(scope.ServiceProvider, logger, config).GetAwaiter().GetResult();

            logger.LogInformation("Seed done.");
        }
        catch (Exception ex)
        {
            logger.LogCritical($"ERROR: {ex.Message}");
        }
    }
    /// <summary>
    /// Seeds a default Administrator user using credentials from environment variables or configuration.
    /// </summary>
    /// <param name="services">Service provider to resolve the UserManager.</param>
    /// <param name="logger">Logger for reporting seeding status.</param>
    /// <param name="configuration">Configuration to access environment-specific secrets.</param>
    private static async Task SeedAdminUserAsync(IServiceProvider services, ILogger logger, IConfiguration configuration)
    {
        var userManager = services.GetRequiredService<UserManager<User>>();

        // Retrieving credentials from IConfiguration (Environment Variables or AppSettings)
        // Azure Key: SeedData__AdminEmail / SeedData__AdminPassword
        string adminEmail = configuration["SeedData:AdminEmail"] ?? "admin@todolist.com";
        string adminPassword = configuration["SeedData:AdminPassword"] ?? "123";

        // Requirement: LINQ Method Syntax to check for existing users
        if (!await userManager.Users.AnyAsync(u => u.Email == adminEmail))
        {
            logger.LogInformation("No administrative user found. Seeding initial admin...");

            var adminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true // Bypassing email verification for the seed user
            };

            // Requirement: Identity implementation for secure password hashing
            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                logger.LogInformation("Administrative user {Email} created successfully.", adminEmail);
            }
            else
            {
                // Detailed error logging in case Identity password requirements are not met
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to seed admin user. Errors: {Errors}", errors);
            }
        }
    }
}