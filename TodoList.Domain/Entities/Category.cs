namespace TodoList.Domain.Entities;

/// <summary>
/// Represents a category to group tasks. 
/// Each category belongs to a specific user.
/// </summary>
public class Category
{
    /// <summary>Unique identifier for the category.</summary>
    public Guid Id { get; set; }
    /// <summary>Display name of the category (max 50 characters).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Foreign Key to the User who owns this category (from ERD).
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    /// <summary>Navigation property to the owning user.</summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Navigation property: List of tasks under this category.
    /// This fixes the "Category does not contain a definition for Tasks" error.
    /// </summary>
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}