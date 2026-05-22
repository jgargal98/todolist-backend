using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure;

/// <summary>
/// Provides utility methods to handle database initialization and data seeding.
/// </summary>
/// <remarks>
/// This class ensures the database schema is up to date via migrations 
/// and populates the system with essential default records.
/// </remarks>
public static class DbInitializer
{
    /// <summary>
    /// Synchronizes the database schema with the current model and seeds initial data.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve scoped dependencies such as <see cref="AppDbContext"/> and <see cref="UserManager{T}"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous database setup process.</returns>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        // Applies any pending migrations to the database
        await context.Database.MigrateAsync();

        // Seeds default administrative data
        // It won't override current user thanks to check
        // await SeedDefaultUserAsync(userManager);
    }

    /// <summary>
    /// Checks if a default administrator exists and creates one if necessary.
    /// </summary>
    /// <param name="userManager">The ASP.NET Core Identity manager used to handle user creation.</param>
    /// <returns>A <see cref="Task"/> that represents the seeding operation.</returns>
    private static async Task SeedDefaultUserAsync(UserManager<User> userManager)
    {
        const string adminEmail = "admin@todolist.com";

        // Check for existing user to avoid duplication
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            // Creates the user with a pre-defined strong password
            await userManager.CreateAsync(admin, "Admin123!");
        }
    }
}