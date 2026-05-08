using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Data.Configurations;

/// <summary>
/// Extends the Identity User configuration to include custom relationships 
/// like the collection of Tasks owned by the user.
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Identity handles most properties (Email, Password, etc.)

        // We only define the relationship with Tasks explicitly
        builder.HasMany(u => u.Tasks)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}