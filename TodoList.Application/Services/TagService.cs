using AutoMapper;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;

namespace TodoList.Application.Services;

/// <summary>
/// Implements tag workflows, orchestrates sanitizations, and isolates core domain object graphs.
/// </summary>
public class TagService(
        ITagRepository tagRepository,
        IUserRepository userRepository,
        IMapper mapper) : ITagService
{
    /// <inheritdoc />
    public async Task<IEnumerable<TagResponse>> GetUserTagsAsync(string userId)
    {
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return Enumerable.Empty<TagResponse>();
        }

        var tags = await tagRepository.GetByUserIdAsync(userId);
        return mapper.Map<IEnumerable<TagResponse>>(tags);
    }

    /// <inheritdoc />
    public async Task<TagResponse?> CreateTagAsync(string userId, CreateTagRequest request)
    {
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return null;
        }

        var newTag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            UserId = userId
        };

        var registered = await tagRepository.AddAsync(newTag);
        if (!registered)
        {
            throw new Exception("Database error: Could not record tag into SQL Server.");
        }

        return mapper.Map<TagResponse>(newTag);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteTagAsync(Guid id, string userId)
    {
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return false;
        }

        var tag = await tagRepository.GetByIdAndUserIdAsync(id, userId);
        if (tag == null)
        {
            return false;
        }

        return await tagRepository.DeleteAsync(id);
    }
}