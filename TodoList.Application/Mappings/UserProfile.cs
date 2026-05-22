using AutoMapper;
using TodoList.Domain.Entities;
using TodoList.Application.DTOs.User;

namespace TodoList.Application.Mappings;

/// <summary>
/// Configuration profile for User-related mappings
/// </summary>
public class UserProfile : Profile
{
    /// <summary>Initializes the mapping configuration for User to UserResponseDto.</summary>
    public UserProfile()
    {
        // Maps ApplicationUser (Database) to UserResponseDto (API Output)
        CreateMap<User, UserResponseDto>();
    }
}