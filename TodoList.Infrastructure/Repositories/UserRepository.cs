using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Infrastructure.Repositories;

/// <summary>
/// Infrastructure-specific implementation of <see cref="IUserRepository"/> 
/// utilizing ASP.NET Core Identity for persistence and security operations.
/// </summary>
/// <remarks>
/// This implementation uses C# 12 primary constructors to inject dependencies.
/// It acts as a wrapper around <see cref="UserManager{TUser}"/>.
/// </remarks>
public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    /// <summary>
    /// Retrieves all registered users from the database, ordered by their username.
    /// </summary>
    /// <returns>A collection of <see cref="User"/> entities.</returns>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await userManager.Users
            .OrderBy(u => u.UserName)
            .ToListAsync();
    }

    /// <summary>
    /// Infrastructure Layer: UserRepository.
    /// Validates user credentials using Identity UserManager.
    /// </summary>
    public async Task<User?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !await userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        return user;
    }

    /// <summary>
    /// Infrastructure Layer: UserRepository.
    /// Creates a new user and handles Identity-specific validation results.
    /// </summary>
    public async Task<bool> CreateAsync(User user, string password)
    {
        var result = await userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    /// <summary>
    /// Updates an existing user's information (e.g., Refresh Tokens, profile data).
    /// </summary>
    /// <param name="user">The user entity with updated values.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the update operation fails in the database.</exception>
    public async Task<bool> UpdateAsync(User user)
    {
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    /// <summary>
    /// Retrieves a user from the database using their email address.
    /// </summary>
    /// <param name="email">The email to search for.</param>
    /// <returns>
    /// A <see cref="User"/> object if found; otherwise, <see langword="null"/>.
    /// </returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        // We use the built-in FindByEmailAsync method from ASP.NET Core Identity's UserManager.
        // This method is optimized and handles the underlying EF Core query.
        return await userManager.FindByEmailAsync(email);
    }
}