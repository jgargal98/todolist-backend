using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Application.Interfaces;

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
    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await userManager.Users
            .OrderBy(u => u.UserName)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<User?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !await userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        return user;
    }

    /// <inheritdoc />
    public async Task<bool> CreateAsync(User user, string password)
    {
        var result = await userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(User user)
    {
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }
}