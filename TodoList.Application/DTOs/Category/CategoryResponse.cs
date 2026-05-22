using System;

namespace TodoList.Application.DTOs.Category;

/// <summary>Data transfer object representing a category for API responses.</summary>
public class CategoryResponse
{
    /// <summary>Unique identifier of the category.</summary>
    public Guid Id { get; set; }
    /// <summary>Display name of the category.</summary>
    public string Name { get; set; } = string.Empty;
}