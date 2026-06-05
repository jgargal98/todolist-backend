namespace TodoList.Domain.Entities;

/// <summary>
/// Represents a pure domain tag abstraction that can be associated with multiple tasks.
/// </summary>
public class Tag
{
    /// <summary>
    /// Unique identifier for the tag entity.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The descriptive name of the tag.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key structural reference to the user identity owning this tag.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Domain navigation reference to the user profile who owns this tag.
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Domain navigation collection managing the structural many-to-many relationship with TaskItems.
    /// </summary>
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}