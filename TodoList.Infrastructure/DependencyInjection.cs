using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;
using TodoList.Infrastructure.Authentication;
using TodoList.Infrastructure.Data;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure;

/// <summary>
/// Centralizes the registration of infrastructure-related services into the dependency injection container.
/// </summary>
/// <remarks>
/// This class follows the Extension Method pattern to keep the API's Program.cs clean 
/// and encapsulates infrastructure details such as Persistence, Identity, and Authentication.
/// </remarks>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure services including Database, Identity, and Authentication.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The application configuration to retrieve settings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddPersistence(configuration)
            .AddIdentityConfiguration()
            .AddAuthenticationInternal(configuration);

        // Data Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    /// <summary>
    /// Configures the database context and persistence layer.
    /// </summary>
    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    /// <summary>
    /// Sets up ASP.NET Core Identity with specific security requirements.
    /// </summary>
    private static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Configures JWT-based authentication and links the Token Provider.
    /// </summary>
    private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
    {

        // Bind JWT settings to the Options pattern
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.ConfigureOptions<JwtSetup>();

        // Register the token generation service (Domain Interface -> Infra Implementation)
        services.AddScoped<ITokenProvider, JwtProvider>();

        // Set up the Authentication Middleware
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

        return services;
    }
}