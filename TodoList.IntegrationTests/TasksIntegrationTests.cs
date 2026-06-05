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

    [Fact]
    public async Task UpdateTask_WithValidData_Returns204NoContent()
    {
        var client = _factory.CreateAuthenticatedClient();
        var taskTitle = $"Update Task {Guid.NewGuid():N}";

        // Arrange: create a task
        var createResponse = await client.PostAsJsonAsync("/api/tasks",
            new CreateTaskRequest { Title = taskTitle });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        // Act: update it
        var updateResponse = await client.PutAsJsonAsync($"/api/tasks/{created!.Id}",
            new UpdateTaskRequest("Updated Title", "Updated Desc", DateTime.UtcNow.AddDays(2), 2, null,
                new List<UpdateSubTaskRequest>(), new List<Guid>()));

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTask_WithExistingId_Returns204NoContent()
    {
        var client = _factory.CreateAuthenticatedClient();
        var taskTitle = $"Delete Task {Guid.NewGuid():N}";

        // Arrange: create a task
        var createResponse = await client.PostAsJsonAsync("/api/tasks",
            new CreateTaskRequest { Title = taskTitle });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        // Act: delete it
        var deleteResponse = await client.DeleteAsync($"/api/tasks/{created!.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateTask_WithInvalidData_Returns400BadRequest()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/tasks",
            new CreateTaskRequest { Title = "" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTask_WithNonExistent_Returns404NotFound()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PutAsJsonAsync($"/api/tasks/{Guid.NewGuid()}",
            new UpdateTaskRequest("Title", null, null, 1, null,
                new List<UpdateSubTaskRequest>(), new List<Guid>()));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_WithNonExistent_Returns404NotFound()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.DeleteAsync($"/api/tasks/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTask_WithCategoryAndTags_Returns200OkWithReferences()
    {
        var client = _factory.CreateAuthenticatedClient();

        // Arrange: create a category
        var catResponse = await client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequest($"Cat_{Guid.NewGuid():N}"));
        catResponse.EnsureSuccessStatusCode();
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryResponse>();

        // Arrange: create a tag
        var tagResponse = await client.PostAsJsonAsync("/api/tags",
            new CreateTagRequest { Name = $"Tag_{Guid.NewGuid():N}" });
        tagResponse.EnsureSuccessStatusCode();
        var tag = await tagResponse.Content.ReadFromJsonAsync<TagResponse>();

        // Act: create task with category and tag
        var taskRequest = new CreateTaskRequest
        {
            Title = $"Full Task {Guid.NewGuid():N}",
            Description = "With category and tag",
            Status = 1,
            DueDate = DateTime.UtcNow.AddDays(3),
            CategoryId = category!.Id,
            TagIds = new List<Guid> { tag!.Id }
        };

        var response = await client.PostAsJsonAsync("/api/tasks", taskRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var taskResponse = await response.Content.ReadFromJsonAsync<TaskResponse>();
        taskResponse.Should().NotBeNull();
        taskResponse!.CategoryId.Should().Be(category.Id);
        taskResponse.Tags.Should().Contain(t => t.Id == tag.Id);
    }

    [Fact]
    public async Task GetTasks_AfterDelete_DoesNotReturnDeletedTask()
    {
        var client = _factory.CreateAuthenticatedClient();
        var taskTitle = $"Gone Task {Guid.NewGuid():N}";

        // Arrange: create and delete a task
        var createResponse = await client.PostAsJsonAsync("/api/tasks",
            new CreateTaskRequest { Title = taskTitle });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        await client.DeleteAsync($"/api/tasks/{created!.Id}");

        // Act
        var response = await client.GetAsync("/api/tasks");
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskResponse>>();

        // Assert
        tasks.Should().NotContain(t => t.Id == created.Id);
    }

    [Fact]
    public async Task CreateTask_WithSubTasks_Returns200OkWithSubTasks()
    {
        var client = _factory.CreateAuthenticatedClient();
        var request = new CreateTaskRequest
        {
            Title = $"SubTask Task {Guid.NewGuid():N}",
            Description = "With subtasks",
            Status = 1,
            SubTasks = new List<CreateSubTaskRequest>
            {
                new() { Title = "Subtask 1", IsDone = false },
                new() { Title = "Subtask 2", IsDone = true }
            }
        };

        var response = await client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var taskResponse = await response.Content.ReadFromJsonAsync<TaskResponse>();
        taskResponse.Should().NotBeNull();
        taskResponse!.SubTasks.Should().HaveCount(2);
        taskResponse.SubTasks.Should().Contain(st => st.Title == "Subtask 1" && !st.IsDone);
        taskResponse.SubTasks.Should().Contain(st => st.Title == "Subtask 2" && st.IsDone);
    }

    [Fact]
    public async Task UpdateTask_WithSubTasksAndTags_Returns204NoContent()
    {
        var client = _factory.CreateAuthenticatedClient();

        // Arrange: create a tag and a task
        var tagResponse = await client.PostAsJsonAsync("/api/tags",
            new CreateTagRequest { Name = $"UpdTag_{Guid.NewGuid():N}" });
        tagResponse.EnsureSuccessStatusCode();
        var tag = await tagResponse.Content.ReadFromJsonAsync<TagResponse>();

        var taskTitle = $"UpdTask {Guid.NewGuid():N}";
        var createResponse = await client.PostAsJsonAsync("/api/tasks",
            new CreateTaskRequest { Title = taskTitle });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        // Act: update with subtasks and a tag
        var updateResponse = await client.PutAsJsonAsync($"/api/tasks/{created!.Id}",
            new UpdateTaskRequest(
                "Updated Title",
                "Updated Desc",
                DateTime.UtcNow.AddDays(5),
                2,
                null,
                new List<UpdateSubTaskRequest>
                {
                    new() { Title = "New Subtask", IsDone = false }
                },
                new List<Guid> { tag!.Id }
            ));

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateTask_WithWhitespaceTitle_Returns400BadRequest()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/tasks",
            new CreateTaskRequest { Title = "   " });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTask_WithWhitespaceTitle_Returns400BadRequest()
    {
        var client = _factory.CreateAuthenticatedClient();
        var taskTitle = $"UpdWsTask {Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("/api/tasks",
            new CreateTaskRequest { Title = taskTitle });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        var updateResponse = await client.PutAsJsonAsync($"/api/tasks/{created!.Id}",
            new UpdateTaskRequest("   ", null, null, 1, null,
                new List<UpdateSubTaskRequest>(), new List<Guid>()));

        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
