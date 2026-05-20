using AutoMapper;
using TodoList.Application.DTOs.Category;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class CategoryProfile : Profile
{
    /// <summary>
    /// Initializes the mapping configurations for Task entities and their respective DTOs.
    /// </summary>
    public CategoryProfile()
    {
        // Primary map configuration for TaskItem to TaskResponse
        CreateMap<Category, CategoryResponse>();
    }
}