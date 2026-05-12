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
/// </remarks>
public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        // Method syntax query as requested
        return await userManager.Users
            .OrderBy(u => u.UserName)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    /// <inheritdoc />
    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await userManager.CheckPasswordAsync(user, password);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(User user)
    {
        // Identity handles the database update transaction
        await userManager.UpdateAsync(user);
    }
}