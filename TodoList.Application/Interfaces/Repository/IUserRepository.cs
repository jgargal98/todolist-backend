using TodoList.Domain.Entities;

namespace TodoList.Application.Interfaces;

/// <summary>
/// Defines the data access contract for User entities using Identity logic.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The matching <see cref="User"/> if found; otherwise, <c>null</c>.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Validates user credentials and returns the User object if successful.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password in plain text.</param>
    /// <returns>The <see cref="User"/> if credentials are valid; otherwise, <c>null</c>.</returns>
    Task<User?> ValidateCredentialsAsync(string email, string password);

    /// <summary>
    /// Creates a new user with the specified password.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <param name="password">The password to hash and store.</param>
    /// <returns><c>true</c> if the user was created successfully; otherwise, <c>false</c>.</returns>
    Task<bool> CreateAsync(User user, string password);

    /// <summary>
    /// Persists changes to an existing user (e.g., updating Refresh Tokens).
    /// </summary>
    /// <param name="user">The user entity with updated values.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    Task<bool> UpdateAsync(User user);

    /// <summary>
    /// Gets a list of all registered users.
    /// </summary>
    /// <returns>A collection of <see cref="User"/> entities ordered by username.</returns>
    Task<IEnumerable<User>> GetAllAsync();
}