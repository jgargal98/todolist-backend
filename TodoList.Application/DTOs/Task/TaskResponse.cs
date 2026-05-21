using TodoList.Application.DTOs.Tag;

namespace TodoList.Application.DTOs.Task;

public class TaskResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Status { get; set; }
    public Guid? CategoryId { get; set; }
    public List<SubTaskResponse> SubTasks { get; set; } = new();
    public List<TagResponse> Tags { get; set; } = new();
}

public class SubTaskResponse
{
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
}