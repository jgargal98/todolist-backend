using TodoList.Application.DTOs.Tag;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Exposes application workflows orchestration contracts and state mapping procedures for Tag entity operations.
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Retrieves all tags for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of <see cref="TagResponse"/> DTOs.</returns>
    Task<IEnumerable<TagResponse>> GetUserTagsAsync(string userId);

    /// <summary>
    /// Creates a new tag for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="request">The tag creation data.</param>
    /// <returns>The created <see cref="TagResponse"/> if successful; otherwise, <c>null</c>.</returns>
    Task<TagResponse?> CreateTagAsync(string userId, CreateTagRequest request);

    /// <summary>
    /// Deletes a tag belonging to the specified user.
    /// </summary>
    /// <param name="id">The unique identifier of the tag.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns><c>true</c> if the deletion succeeds; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteTagAsync(Guid id, string userId);
}