using TodoList.Application.DTOs.Category;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Defines the application business logic workflow orchestrations for categories.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Coordinates user context validation and maps categories into decoupled data transfer responses.
    /// </summary>
    Task<IEnumerable<CategoryResponse>> GetUserCategoriesAsync(string userId);

    /// <summary>
    /// Orchestrates defensive user checking, text sanitization, and category allocation workflows.
    /// </summary>
    Task<CategoryResponse?> CreateCategoryAsync(string userId, CreateCategoryRequest request);

    /// <summary>
    /// Coordinates targeted resource state modifications and enforces isolation boundaries.
    /// </summary>
    Task<bool> UpdateCategoryAsync(Guid id, string userId, UpdateCategoryRequest request);

    /// <summary>
    /// Coordinates the permanent removal of a scoped category resource asset.
    /// </summary>
    Task<bool> DeleteCategoryAsync(Guid id, string userId);
}