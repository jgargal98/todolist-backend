using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Data.Configurations;

/// <summary>
/// Database mapping for TaskItem.
/// Implements the JSON storage for SubTasks and specific column names from ERD.
/// </summary>
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Mapping to the exact names in your ERD
        builder.Property(t => t.Description)
            .HasColumnName("desc")
            .HasMaxLength(1000);

        builder.Property(t => t.DueDate)
            .HasColumnName("duedate");

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .IsRequired();

        // JSON COLUMN CONFIGURATION (No extra table)
        builder.OwnsMany(t => t.SubTasks, navigationBuilder =>
        {
            navigationBuilder.ToJson("subTask"); // Column name in SQL will be 'subTask'
        });

        // Relationships
        builder.HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}