using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList.Domain.Entities;

/// <summary>
/// Represents a tag that can be associated with multiple tasks.
/// </summary>
public class Tag
{
    /// <summary>
    /// Unique identifier for the tag.
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Name of the tag.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key referencing the owner of the tag.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property for the user who owns this tag.
    /// </summary>
    [ForeignKey("IdUser")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Navigation property for the many-to-many relationship with TaskItems.
    /// </summary>
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}