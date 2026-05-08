namespace TodoList.Application.DTOs.User;

public record UserResponseDto
{
    public UserResponseDto() { }

    public Guid Id { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public int TotalTasks { get; init; }
}