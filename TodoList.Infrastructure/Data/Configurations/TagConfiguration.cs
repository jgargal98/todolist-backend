using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(50);

        // As per ERD: Tag has a UserId
        builder.Property(t => t.UserId).IsRequired();

        // Many-to-Many Relationship
        builder.HasMany(t => t.Tasks)
            .WithMany(tk => tk.Tags)
            .UsingEntity(j => j.ToTable("TaskTags"));
    }
}