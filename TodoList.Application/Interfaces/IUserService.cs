using TodoList.Application.DTOs.User;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Service interface for user-related business logic.
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetUsersAsync();
}