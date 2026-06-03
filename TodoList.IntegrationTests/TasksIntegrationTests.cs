namespace TodoList.IntegrationTests;

public sealed class TasksIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public TasksIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateTask_WithValidData_Returns200OkWithTask()
    {
        var client = _factory.CreateAuthenticatedClient();
        var request = new CreateTaskRequest
        {
            Title = $"Integration Task {Guid.NewGuid():N}",
            Description = "Created during integration test",
            Status = 1,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var response = await client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var taskResponse = await response.Content.ReadFromJsonAsync<TaskResponse>();
        taskResponse.Should().NotBeNull();
        taskResponse!.Title.Should().Be(request.Title);
        taskResponse.Description.Should().Be(request.Description);
        taskResponse.Status.Should().Be(request.Status);
        taskResponse.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetTasks_Returns200OkWithList()
    {
        var client = _factory.CreateAuthenticatedClient();
        var taskTitle = $"List Task {Guid.NewGuid():N}";

        // Arrange: create a task first
        var createResponse = await client.PostAsJsonAsync("/api/tasks",
            new CreateTaskRequest { Title = taskTitle });
        createResponse.EnsureSuccessStatusCode();

        // Act
        var response = await client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskResponse>>();
        tasks.Should().NotBeNull();
        tasks.Should().Contain(t => t.Title == taskTitle);
    }

    [Fact]
    public async Task AnyEndpoint_WithoutAuth_Returns401Unauthorized()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
