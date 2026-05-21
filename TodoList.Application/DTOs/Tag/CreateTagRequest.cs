namespace TodoList.Application.DTOs.Tag;

/// <summary>
/// Data transfer object for creating a pristine new tag resource.
/// </summary>
public class CreateTagRequest
{
    /// <summary>
    /// Gets or sets the descriptive name of the tag.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}