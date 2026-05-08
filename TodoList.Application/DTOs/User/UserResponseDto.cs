namespace TodoList.Application.DTOs.User;

public record UserResponseDto(
    Guid Id,
    string Email,
    string FullName,
    int TotalTasks
);