namespace TodoList.UnitTests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ITagRepository> _tagRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _taskRepoMock = new Mock<ITaskRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _tagRepoMock = new Mock<ITagRepository>();
        _mapperMock = new Mock<IMapper>();

        _mapperMock.Setup(m => m.Map<TaskResponse>(It.IsAny<TaskItem>()))
            .Returns((TaskItem t) => new TaskResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,
                CategoryId = t.CategoryId,
                SubTasks = t.SubTasks.Select(st => new SubTaskResponse { Title = st.Title, IsDone = st.IsDone }).ToList(),
                Tags = t.Tags.Select(tag => new TagResponse { Id = tag.Id, Name = tag.Name, UserId = tag.UserId }).ToList()
            });

        _mapperMock.Setup(m => m.Map<IEnumerable<TaskResponse>>(It.IsAny<IEnumerable<TaskItem>>()))
            .Returns((IEnumerable<TaskItem> tasks) => tasks?.Select(t => new TaskResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,
                CategoryId = t.CategoryId,
                SubTasks = t.SubTasks.Select(st => new SubTaskResponse { Title = st.Title, IsDone = st.IsDone }).ToList(),
                Tags = t.Tags.Select(tag => new TagResponse { Id = tag.Id, Name = tag.Name, UserId = tag.UserId }).ToList()
            }).ToList() ?? new List<TaskResponse>());

        _sut = new TaskService(
            _taskRepoMock.Object,
            _userRepoMock.Object,
            _tagRepoMock.Object,
            _mapperMock.Object);
    }

    private static string UserId => "user-1";

    private static CreateTaskRequest ValidCreateRequest => new()
    {
        Title = "Test Task",
        Description = "Description",
        DueDate = DateTime.UtcNow.AddDays(1),
        Status = 1,
        SubTasks = new List<CreateSubTaskRequest>
        {
            new() { Title = "Subtask 1", IsDone = false }
        },
        TagIds = new List<Guid> { Guid.NewGuid() }
    };

    private static UpdateTaskRequest ValidUpdateRequest => new(
        "Updated Title",
        "Updated Description",
        DateTime.UtcNow.AddDays(2),
        2,
        null,
        new List<UpdateSubTaskRequest>(),
        new List<Guid>());

    private static Tag CreateTag(Guid id, string userId) => new()
    {
        Id = id,
        Name = $"Tag-{id}",
        UserId = userId
    };

    private static List<User> UsersWith(string userId) =>
        [new() { Id = userId, Email = "u@test.com", UserName = "u" }];

    [Fact]
    public async Task CreateTaskAsync_WithValidData_ReturnsTaskResponse()
    {
        var tagId = Guid.NewGuid();
        var request = ValidCreateRequest;
        request.TagIds = [tagId];

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.GetTagsByIdsAsync(It.IsAny<List<Guid>>(), UserId))
            .ReturnsAsync((List<Guid> ids, string uid) => ids.Select(id => CreateTag(id, uid)).ToList());
        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(true);

        var result = await _sut.CreateTaskAsync(UserId, request);

        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Single(result.SubTasks);
        Assert.Single(result.Tags);
    }

    [Fact]
    public async Task CreateTaskAsync_WhenUserNotFound_ReturnsNull()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<User>());

        var result = await _sut.CreateTaskAsync("unknown-user", ValidCreateRequest);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTaskAsync_WithNullUsers_ReturnsNull()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync((IEnumerable<User>)null!);

        var result = await _sut.CreateTaskAsync(UserId, ValidCreateRequest);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTaskAsync_WithInvalidTags_ReturnsNull()
    {
        var tagId = Guid.NewGuid();
        var request = ValidCreateRequest;
        request.TagIds = [tagId];

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.GetTagsByIdsAsync(It.IsAny<List<Guid>>(), UserId))
            .ReturnsAsync(Enumerable.Empty<Tag>());

        var result = await _sut.CreateTaskAsync(UserId, request);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTaskAsync_WhenAddFails_ThrowsException()
    {
        var request = ValidCreateRequest;
        request.TagIds = [];

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _sut.CreateTaskAsync(UserId, request));
        Assert.Contains("Database error", ex.Message);
    }

    [Fact]
    public async Task CreateTaskAsync_WithNoTags_ReturnsTaskResponse()
    {
        var request = ValidCreateRequest;
        request.TagIds = [];

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(true);

        var result = await _sut.CreateTaskAsync(UserId, request);

        Assert.NotNull(result);
        Assert.Empty(result.Tags);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithValidData_ReturnsTrue()
    {
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Old Title",
            UserId = UserId,
            SubTasks = new List<SubTask>(),
            Tags = new List<Tag>()
        };

        _taskRepoMock.Setup(r => r.GetByIdWithTagsAsync(taskId, UserId))
            .ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateTaskAsync(taskId, UserId, ValidUpdateRequest);

        Assert.True(result);
        Assert.Equal("Updated Title", task.Title);
    }

    [Fact]
    public async Task UpdateTaskAsync_WhenTaskNotFound_ReturnsFalse()
    {
        _taskRepoMock.Setup(r => r.GetByIdWithTagsAsync(It.IsAny<Guid>(), UserId))
            .ReturnsAsync((TaskItem?)null);

        var result = await _sut.UpdateTaskAsync(Guid.NewGuid(), UserId, ValidUpdateRequest);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTaskAsync_WhenTaskBelongsToOtherUser_ReturnsFalse()
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Other's Task",
            UserId = "other-user",
            SubTasks = new List<SubTask>(),
            Tags = new List<Tag>()
        };

        _taskRepoMock.Setup(r => r.GetByIdWithTagsAsync(task.Id, UserId))
            .ReturnsAsync(task);

        var result = await _sut.UpdateTaskAsync(task.Id, UserId, ValidUpdateRequest);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithCrossTenantTags_ReturnsFalse()
    {
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Title",
            UserId = UserId,
            SubTasks = new List<SubTask>(),
            Tags = new List<Tag>()
        };

        var request = ValidUpdateRequest with
        {
            TagIds = [Guid.NewGuid()]
        };

        _taskRepoMock.Setup(r => r.GetByIdWithTagsAsync(taskId, UserId))
            .ReturnsAsync(task);
        _tagRepoMock.Setup(r => r.GetTagsByIdsAsync(It.IsAny<List<Guid>>(), UserId))
            .ReturnsAsync(Enumerable.Empty<Tag>());

        var result = await _sut.UpdateTaskAsync(taskId, UserId, request);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_WithExistingTask_ReturnsTrue()
    {
        var taskId = Guid.NewGuid();
        var tasks = new List<TaskItem>
        {
            new() { Id = taskId, Title = "Task", UserId = UserId }
        };

        _taskRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(tasks);
        _taskRepoMock.Setup(r => r.DeleteAsync(taskId))
            .ReturnsAsync(true);

        var result = await _sut.DeleteTaskAsync(taskId, UserId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_WhenNoUserTasks_ReturnsFalse()
    {
        _taskRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync((IEnumerable<TaskItem>)null!);

        var result = await _sut.DeleteTaskAsync(Guid.NewGuid(), UserId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_WhenTaskNotFound_ReturnsFalse()
    {
        _taskRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(new List<TaskItem>());

        var result = await _sut.DeleteTaskAsync(Guid.NewGuid(), UserId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_WhenDeleteFails_ThrowsException()
    {
        var taskId = Guid.NewGuid();
        var tasks = new List<TaskItem>
        {
            new() { Id = taskId, Title = "Task", UserId = UserId }
        };

        _taskRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(tasks);
        _taskRepoMock.Setup(r => r.DeleteAsync(taskId))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _sut.DeleteTaskAsync(taskId, UserId));
        Assert.Contains("Database error", ex.Message);
    }

    [Fact]
    public async Task GetTasksByUserIdAsync_ReturnsMappedTasks()
    {
        var tasks = new List<TaskItem>
        {
            new() { Id = Guid.NewGuid(), Title = "Task 1", UserId = UserId, SubTasks = new List<SubTask>(), Tags = new List<Tag>() },
            new() { Id = Guid.NewGuid(), Title = "Task 2", UserId = UserId, SubTasks = new List<SubTask>(), Tags = new List<Tag>() }
        };

        _taskRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(tasks);

        var result = await _sut.GetTasksByUserIdAsync(UserId);

        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.Title == "Task 1");
    }

    [Fact]
    public async Task GetTasksByUserIdAsync_WhenNullFromRepo_ReturnsEmpty()
    {
        _taskRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync((IEnumerable<TaskItem>)null!);

        var result = await _sut.GetTasksByUserIdAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTasksByUserIdAsync_WithNoTasks_ReturnsEmpty()
    {
        _taskRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(Enumerable.Empty<TaskItem>());

        var result = await _sut.GetTasksByUserIdAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateTaskAsync_TrimsStrings()
    {
        var request = ValidCreateRequest;
        request.Title = "  Trimmed Title  ";
        request.Description = "  Trimmed Desc  ";
        request.TagIds = [];

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));

        TaskItem? captured = null;
        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .Callback<TaskItem>(t => captured = t)
            .ReturnsAsync(true);

        await _sut.CreateTaskAsync(UserId, request);

        Assert.NotNull(captured);
        _taskRepoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_WithCategoryId_PassesItThrough()
    {
        var categoryId = Guid.NewGuid();
        var request = ValidCreateRequest;
        request.CategoryId = categoryId;
        request.TagIds = [];

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(true);

        TaskItem? captured = null;
        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .Callback<TaskItem>(t => captured = t)
            .ReturnsAsync(true);

        await _sut.CreateTaskAsync(UserId, request);

        Assert.NotNull(captured);
        Assert.Equal(categoryId, captured!.CategoryId);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithSubTasks_MapsThemCorrectly()
    {
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Old",
            UserId = UserId,
            SubTasks = new List<SubTask>(),
            Tags = new List<Tag>()
        };

        var request = ValidUpdateRequest with
        {
            SubTasks = new List<UpdateSubTaskRequest>
            {
                new() { Title = "New Subtask", IsDone = true }
            }
        };

        _taskRepoMock.Setup(r => r.GetByIdWithTagsAsync(taskId, UserId))
            .ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateTaskAsync(taskId, UserId, request);

        Assert.True(result);
        Assert.Single(task.SubTasks);
        Assert.Equal("New Subtask", task.SubTasks[0].Title);
        Assert.True(task.SubTasks[0].IsDone);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithNullTagIds_DoesNotSyncTags()
    {
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Old",
            UserId = UserId,
            SubTasks = new List<SubTask>(),
            Tags = new List<Tag>()
        };

        _taskRepoMock.Setup(r => r.GetByIdWithTagsAsync(taskId, UserId))
            .ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(true);

        var request = ValidUpdateRequest with { TagIds = null! };

        var result = await _sut.UpdateTaskAsync(taskId, UserId, request);

        Assert.True(result);
        _tagRepoMock.Verify(r => r.GetTagsByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<string>()), Times.Never);
    }
}
