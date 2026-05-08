using Microsoft.AspNetCore.Identity;

namespace TodoList.Domain.Entities;

/// <summary>
/// Represents the custom user entity in the system, extending IdentityUser.
/// </summary>
public class User : IdentityUser
{
    // Custom properties can be added here in the future

    /// <summary>
    /// Navigation property for the tasks owned by this user.
    /// </summary>
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    /// <summary>
    /// Navigation property for the tags created by this user.
    /// </summary>
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    /// <summary>
    /// Navigation property for the categories created by this user.
    /// </summary>
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}