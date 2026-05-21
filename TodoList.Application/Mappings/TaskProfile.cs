using AutoMapper;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.DTOs.Task;
using TodoList.Domain.Entities;

namespace TodoList.Application.Mappings;

public class TaskProfile : Profile
{
    /// <summary>
    /// Initializes the mapping configurations for Task entities and their respective DTOs.
    /// </summary>
    public TaskProfile()
    {
        // Primary map configuration for TaskItem to TaskResponse
        CreateMap<TaskItem, TaskResponse>();

        // Map configuration for SubTask to SubTaskResponse
        CreateMap<SubTask, SubTaskResponse>();

        CreateMap<Tag, TagResponse>();
    }
}