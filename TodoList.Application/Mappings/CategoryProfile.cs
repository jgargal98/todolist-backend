using AutoMapper;
using TodoList.Application.DTOs.Category;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

/// <summary>AutoMapper profile for Category entity to DTO mappings.</summary>
public class CategoryProfile : Profile
{
    /// <summary>
    /// Initializes the mapping configuration for Category entities to CategoryResponse DTOs.
    /// </summary>
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponse>();
    }
}