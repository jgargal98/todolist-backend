using AutoMapper;
using TodoList.Application.DTOs.Tag;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TagProfile : Profile
{
    /// <summary>
    /// Initializes the mapping configurations for Task entities and their respective DTOs.
    /// </summary>
    public TagProfile()
    {
        // Primary map configuration for TaskItem to TaskResponse
        CreateMap<Tag, TagResponse>();

    }
}