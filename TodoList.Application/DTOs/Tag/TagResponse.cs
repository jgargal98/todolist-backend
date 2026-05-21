namespace TodoList.Application.DTOs.Tag;

/// <summary>
/// Data transfer object representing the structural query response schema of a tag asset.
/// </summary>
public class TagResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the tag.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the descriptive name of the tag.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user identity string context owner of this tag.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}