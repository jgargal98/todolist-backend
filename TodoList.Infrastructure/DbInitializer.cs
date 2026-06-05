using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure;

/// <summary>
/// Provides utility methods to handle database initialization via migrations.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Applies pending migrations to keep the database schema up to date.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve scoped dependencies such as <see cref="AppDbContext"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous migration operation.</returns>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();
    }
}