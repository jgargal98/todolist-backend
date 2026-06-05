namespace TodoList.Domain.Entities;

/// <summary>
/// Main Task entity based on the ERD.
/// </summary>
public class TaskItem
{
    /// <summary>Unique identifier for the task.</summary>
    public Guid Id { get; set; }
    /// <summary>Title or short description of the task.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Mapped to 'desc' in ERD</summary>
    public string? Description { get; set; }

    /// <summary>Mapped to 'duedate' in ERD</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Mapped to 'status' in ERD</summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// Stored as JSON in the database as per ERD 'subTask: JSON'.
    /// </summary>
    public List<SubTask> SubTasks { get; set; } = new List<SubTask>();

    /// <summary>Foreign key to the owning user.</summary>
    public string UserId { get; set; } = string.Empty;
    /// <summary>Navigation property to the owning user.</summary>
    public virtual User User { get; set; } = null!;

    /// <summary>Foreign key to an optional category.</summary>
    public Guid? CategoryId { get; set; }
    /// <summary>Navigation property to the assigned category.</summary>
    public virtual Category? Category { get; set; }

    /// <summary>Collection of tags associated with this task (many-to-many).</summary>
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}