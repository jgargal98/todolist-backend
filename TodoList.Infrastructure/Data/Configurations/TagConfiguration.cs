using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="Tag"/> entity.
/// </summary>
/// <remarks>
/// This class defines the database schema, constraints, and relationships 
/// for tags using the Fluent API approach.
/// </remarks>
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    /// <summary>
    /// Configures the database mapping for the Tag entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        // Table name mapping
        builder.ToTable("Tags");

        // Primary Key configuration
        builder.HasKey(t => t.Id);

        // Property constraints
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        // Foreign Key property for the owner of the tag
        builder.Property(t => t.UserId)
            .IsRequired();

        /// <summary>
        /// Relationship Mapping: Many-to-Many
        /// Configures the relationship between Tags and Tasks.
        /// A join table named "TaskTags" is automatically managed by EF Core.
        /// </summary>
        builder.HasMany(t => t.Tasks)
            .WithMany(tk => tk.Tags)
            .UsingEntity(j => j.ToTable("TaskTags"));
    }
}