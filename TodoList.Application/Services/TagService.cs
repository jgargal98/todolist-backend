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
        // 1. Verify target user record integrity exists inside the database context
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return Enumerable.Empty<TagResponse>();
        }

        // 2. Fetch all domains and directly leverage mapping schemas
        var tags = await tagRepository.GetByUserIdAsync(userId);
        return mapper.Map<IEnumerable<TagResponse>>(tags);
    }

    /// <inheritdoc />
    public async Task<TagResponse?> CreateTagAsync(string userId, CreateTagRequest request)
    {
        // 1. Verify target user record integrity exists inside the database context
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return null;
        }

        // 2. Map and strictly sanitize structural strings directly into a pristine Domain Entity instance
        var newTag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            UserId = userId
        };

        // 3. Call infrastructure storage repository layer to commit structural entities
        var registered = await tagRepository.AddAsync(newTag);
        if (!registered)
        {
            throw new Exception("Database error: Could not record tag into SQL Server.");
        }

        // 4. Transform Domain Graph to simple DTO to fully break serialization loops
        return mapper.Map<TagResponse>(newTag);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteTagAsync(Guid id, string userId)
    {
        // 1. Verify target user record integrity exists inside the database context
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return false;
        }

        // 2. Locate target entity preventing illegal record manipulations from third party contexts
        var tag = await tagRepository.GetByIdAndUserIdAsync(id, userId);
        if (tag == null)
        {
            return false;
        }

        // 3. Request resource purge execution passing directly the validated Guid identifier
        return await tagRepository.DeleteAsync(id);
    }
}