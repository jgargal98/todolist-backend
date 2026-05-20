namespace TodoList.Domain.Entities;

/// <summary>
/// Main Task entity based on the ERD.
/// </summary>
public class TaskItem
{
    public Guid Id { get; set; }
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

    // Foreign Keys & Navigation
    public string UserId { get; set; } = string.Empty;
    public virtual User User { get; set; } = null!;

    public Guid? CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}