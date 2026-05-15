using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces;

/// <summary>
/// Defines the data access contract for User entities using Identity logic.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique email address.
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Validates user credentials and returns the User object if successful.
    /// </summary>
    Task<User?> ValidateCredentialsAsync(string email, string password);

    /// <summary>
    /// Creates a new user with the specified password.
    /// </summary>
    Task<bool> CreateAsync(User user, string password);

    /// <summary>
    /// Persists changes to an existing user (e.g., updating Refresh Tokens).
    /// </summary>
    Task<bool> UpdateAsync(User user);

    /// <summary>
    /// Gets a list of all registered users.
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync();
}