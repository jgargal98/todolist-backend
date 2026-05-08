using TodoList.Application.DTOs.User;
using TodoList.Domain.Entities;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Service interface for user-related business logic.
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetUsersAsync();
}