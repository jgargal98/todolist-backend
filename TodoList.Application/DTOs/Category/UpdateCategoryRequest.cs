namespace TodoList.Application.DTOs.Category;

/// <summary>
/// Data transfer object for updating an existing category.
/// </summary>
/// <param name="Name">Updated display name of the category.</param>
public record UpdateCategoryRequest(string Name);