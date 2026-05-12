using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces;

public interface IUserRepository
{
    // The repository always works with Domain Entities
    // Returns a list of all users (testing)
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// Retrieves a user entity by its email address.
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Checks if the provided password is valid for the given user.
    /// </summary>
    Task<bool> CheckPasswordAsync(User user, string password);

    /// <summary>
    /// Persists changes to the user entity, including security-related fields.
    /// </summary>
    Task UpdateAsync(User user);
}