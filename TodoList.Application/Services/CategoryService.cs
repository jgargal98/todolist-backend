using AutoMapper;
using TodoList.Application.DTOs.Category;
using TodoList.Domain.Interfaces;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;

namespace TodoList.Application.Services;

/// <summary>
/// Implements category workflows, orchestrates sanitizations, and isolates core domain object graphs.
/// </summary>
public class CategoryService(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IMapper mapper) : ICategoryService
{

    /// <inheritdoc />
    public async Task<IEnumerable<CategoryResponse>> GetUserCategoriesAsync(string userId)
    {
        // 1. Verify target user record integrity exists inside the database context
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return Enumerable.Empty<CategoryResponse>();
        }

        // 2. Fetch all domains and directly leverage mapping schemas
        var categories = await categoryRepository.GetByUserIdAsync(userId);
        return mapper.Map<IEnumerable<CategoryResponse>>(categories);
    }

    /// <inheritdoc />
    public async Task<CategoryResponse?> CreateCategoryAsync(string userId, CreateCategoryRequest request)
    {
        // 1. Verify target user record integrity exists inside the database context
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return null;
        }

        // 2. Map and strictly sanitize structural strings directly into a pristine Domain Entity instance
        var newCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            UserId = userId
        };

        // 3. Call infrastructure storage repository layer to commit structural entities
        var registered = await categoryRepository.AddAsync(newCategory);

        if (!registered)
        {
            throw new Exception("Database error: Could not record category into SQL Server.");
        }

        // 4. Transform Domain Graph to simple DTO to fully break serialization loops
        return mapper.Map<CategoryResponse>(newCategory);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateCategoryAsync(Guid id, string userId, UpdateCategoryRequest request)
    {
        // 1. Verify target user record integrity exists inside the database context
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return false;
        }

        // 2. Query target entity ensuring cross-tenant boundaries are strictly guarded
        var category = await categoryRepository.GetByIdAndUserIdAsync(id, userId);
        if (category == null)
        {
            return false;
        }

        // 3. Apply sanitized structural mutations directly over the monitored tracker instance
        category.Name = request.Name.Trim();

        // 4. Force state tracking commit pipelines
        var updated = await categoryRepository.UpdateAsync(category);
        if (!updated)
        {
            throw new Exception("Database error: Could not update category into SQL Server.");
        }

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteCategoryAsync(Guid id, string userId)
    {
        // 1. Verify target user record integrity exists inside the database context
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return false;
        }

        // 2. Locate target entity preventing illegal record manipulations from third party contexts
        var category = await categoryRepository.GetByIdAndUserIdAsync(id, userId);
        if (category == null)
        {
            return false;
        }

        // 3. Request resource purge execution
        var deleted = await categoryRepository.DeleteAsync(category);
        if (!deleted)
        {
            throw new Exception("Database error: Could not delete category from SQL Server.");
        }

        return true;
    }
}