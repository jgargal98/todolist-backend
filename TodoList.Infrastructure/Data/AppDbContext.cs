using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Data;

/// <summary>
/// Database context for the application, managing Identity and Domain entities mapping.
/// </summary>
/// <remarks>
/// Initializes a new instance of the AppDbContext.
/// </remarks>
/// <param name="options">The options to be used by a DbContext.</param>
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{

    /// <summary>
    /// Gets or sets the Tasks table.
    /// </summary>
    public DbSet<TaskItem> Tasks { get; set; }

    /// <summary>
    /// Gets or sets the Tags table.
    /// </summary>
    public DbSet<Tag> Tags { get; set; }

    /// <summary>
    /// Gets or sets the Categories table.
    /// </summary>
    public DbSet<Category> Categories { get; set; }

    /// <summary>
    /// Configures the database schema and relationships using Fluent API.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // This line scans the current project for all IEntityTypeConfiguration classes
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}