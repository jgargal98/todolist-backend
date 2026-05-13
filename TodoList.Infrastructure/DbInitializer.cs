using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure;

/// <summary>
/// Handles initial database creation and administrative data seeding without Migrations.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Ensures the database exists and seeds the initial admin user.
    /// </summary>
    /// <summary>
    /// Initializes the database by setting the data directory, applying migrations, and seeding data.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies.</param>
    /// <param name="dataPath">The absolute path where the database files should be stored.</param>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await context.Database.MigrateAsync();
        await SeedDefaultUserAsync(userManager);
    }

    private static async Task SeedDefaultUserAsync(UserManager<User> userManager)
    {
        const string adminEmail = "admin@todolist.com";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, "Admin123!");
        }
    }
}