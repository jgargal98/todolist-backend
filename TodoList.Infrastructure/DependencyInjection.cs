using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Data;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure;

/// <summary>
/// Extension class to centralize all Infrastructure-related dependency registrations.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configures Database, Identity, and Repositories for the Infrastructure layer.
    /// </summary>
    /// <param name="services">The service collection from Program.cs.</param>
    /// <param name="configuration">The application configuration to access connection strings.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. DATABASE CONFIGURATION
        // Fetches the connection string from appsettings.json or Azure Environment Variables
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // 2. IDENTITY CONFIGURATION
        // IMPORTANT: Requires 'Microsoft.AspNetCore.Identity.EntityFrameworkCore' NuGet package
        services.AddIdentity<User, IdentityRole>(options =>
        {
            // Optional: Configure password requirements here to match your seed password
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // 3. REPOSITORY REGISTRATION
        // Registering implementations against their Domain interfaces
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}