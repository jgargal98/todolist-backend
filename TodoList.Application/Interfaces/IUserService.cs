using TodoList.Application.DTOs.User;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Defines the contract for business logic operations related to user management.
/// </summary>
/// <remarks>
/// This service acts as an intermediary between the infrastructure layer and the API,
/// transforming domain entities into specialized Data Transfer Objects (DTOs).
/// </remarks>
public interface IUserService
{
    /// <summary>
    /// Retrieves all registered users in the system.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="UserResponseDto"/> representing the public profiles of all users.
    /// </returns>
    /// <remarks>
    /// The implementation should handle the mapping from the internal User model 
    /// to the optimized response format.
    /// </remarks>
    Task<IEnumerable<UserResponseDto>> GetUsersAsync();
}