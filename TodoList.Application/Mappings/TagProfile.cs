using AutoMapper;
using TodoList.Application.DTOs.Tag;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

/// <summary>AutoMapper profile for Tag entity to DTO mappings.</summary>
public class TagProfile : Profile
{
    /// <summary>
    /// Initializes the mapping configuration for Tag entities to TagResponse DTOs.
    /// </summary>
    public TagProfile()
    {
        CreateMap<Tag, TagResponse>();
    }
}