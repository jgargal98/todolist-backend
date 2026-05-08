namespace TodoList.Infrastructure.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

/// <summary>
/// Implementation of user-related data operations using ASP.NET Identity.
/// </summary>
public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    /// <summary>
    /// Retrieves all registered users using LINQ Method Syntax.
    /// </summary>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        // REQUIREMENT: Method syntax queries
        return await userManager.Users
            .OrderBy(u => u.UserName)
            .ToListAsync();
    }
}