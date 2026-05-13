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
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        // 1. Create the database and tables if they don't exist (No migrations needed)
        await context.Database.EnsureCreatedAsync();

        // 2. Seed Admin User
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