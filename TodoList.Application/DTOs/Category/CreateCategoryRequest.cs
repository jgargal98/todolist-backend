namespace TodoList.Application.DTOs.Category;

/// <summary>
/// Data transfer object for creating a new category.
/// </summary>
/// <param name="Name">Display name of the category.</param>
public record CreateCategoryRequest(string Name);