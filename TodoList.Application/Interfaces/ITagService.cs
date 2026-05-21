using TodoList.Application.DTOs.Tag;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Exposes application workflows orchestration contracts and state mapping procedures for Tag entity operations.
/// </summary>
public interface ITagService
{
    Task<IEnumerable<TagResponse>> GetUserTagsAsync(string userId);
    Task<TagResponse?> CreateTagAsync(string userId, CreateTagRequest request);
    Task<bool> DeleteTagAsync(Guid id, string userId);
}