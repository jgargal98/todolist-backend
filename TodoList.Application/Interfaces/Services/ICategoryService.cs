using TodoList.Application.DTOs.Category;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Defines the application business logic workflows for category operations.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Retrieves all categories associated with the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of <see cref="CategoryResponse"/> DTOs.</returns>
    Task<IEnumerable<CategoryResponse>> GetUserCategoriesAsync(string userId);

    /// <summary>
    /// Creates a new category for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="request">The category creation data.</param>
    /// <returns>The created <see cref="CategoryResponse"/> if successful; otherwise, <c>null</c>.</returns>
    Task<CategoryResponse?> CreateCategoryAsync(string userId, CreateCategoryRequest request);

    /// <summary>
    /// Updates an existing category after verifying user ownership.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="request">The updated category data.</param>
    /// <returns><c>true</c> if the update succeeds; otherwise, <c>false</c>.</returns>
    Task<bool> UpdateCategoryAsync(Guid id, string userId, UpdateCategoryRequest request);

    /// <summary>
    /// Deletes a category belonging to the specified user.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns><c>true</c> if the deletion succeeds; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteCategoryAsync(Guid id, string userId);
}