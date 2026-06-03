using AutoMapper;
using TodoList.Application.DTOs.Category;
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
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return Enumerable.Empty<CategoryResponse>();
        }

        var categories = await categoryRepository.GetByUserIdAsync(userId);
        return mapper.Map<IEnumerable<CategoryResponse>>(categories);
    }

    /// <inheritdoc />
    public async Task<CategoryResponse?> CreateCategoryAsync(string userId, CreateCategoryRequest request)
    {
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return null;
        }

        var newCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            UserId = userId
        };

        var registered = await categoryRepository.AddAsync(newCategory);

        if (!registered)
        {
            throw new Exception("Database error: Could not record category into SQL Server.");
        }

        return mapper.Map<CategoryResponse>(newCategory);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateCategoryAsync(Guid id, string userId, UpdateCategoryRequest request)
    {
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return false;
        }

        var category = await categoryRepository.GetByIdAndUserIdAsync(id, userId);
        if (category == null)
        {
            return false;
        }

        category.Name = request.Name.Trim();

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
        var users = await userRepository.GetAllAsync();
        if (users is null || !users.Any(u => u.Id == userId))
        {
            return false;
        }

        var category = await categoryRepository.GetByIdAndUserIdAsync(id, userId);
        if (category == null)
        {
            return false;
        }

        var deleted = await categoryRepository.DeleteAsync(category);
        if (!deleted)
        {
            throw new Exception("Database error: Could not delete category from SQL Server.");
        }

        return true;
    }
}