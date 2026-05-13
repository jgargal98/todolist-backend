namespace TodoList.Application.DTOs.User;

/// <summary>
/// Data Transfer Object representing the public user information returned by the API.
/// </summary>
/// <remarks>
/// This record is used to encapsulate user data for responses, 
/// ensuring sensitive information like password hashes is not exposed.
/// </remarks>
public record UserResponseDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserResponseDto"/> record.
    /// </summary>
    public UserResponseDto() { }

    /// <summary>
    /// Gets the unique identifier for the user.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the user's primary email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets the unique display name or handle of the user.
    /// </summary>
    public required string UserName { get; init; }

    /// <summary>
    /// Gets the total count of tasks associated with this user.
    /// </summary>
    public int TotalTasks { get; init; }
}