using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TodoList.Application.Mappings;

namespace TodoList.UnitTests.Mappings;

public class MappingProfileTests
{
    private static IMapper CreateMapper(Action<IMapperConfigurationExpression> configure)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(configure);
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact]
    public void UserProfile_Configuration_IsValid()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<UserProfile>());
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void TaskProfile_Configuration_IsValid()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<TaskProfile>());
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void TagProfile_Configuration_IsValid()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<TagProfile>());
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void CategoryProfile_Configuration_IsValid()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<CategoryProfile>());
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void AllProfiles_Combined_CanCreateMapper()
    {
        var mapper = CreateMapper(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<TaskProfile>();
            cfg.AddProfile<TagProfile>();
            cfg.AddProfile<CategoryProfile>();
        });
        Assert.NotNull(mapper);
    }

    [Fact]
    public void UserToUserResponseDto_MapsCorrectly()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<UserProfile>());

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "testuser"
        };

        var dto = mapper.Map<UserResponseDto>(user);

        Assert.Equal(user.Id, dto.Id.ToString());
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.UserName, dto.UserName);
    }

    [Fact]
    public void CategoryToCategoryResponse_MapsCorrectly()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<CategoryProfile>());

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Work"
        };

        var dto = mapper.Map<CategoryResponse>(category);

        Assert.Equal(category.Id, dto.Id);
        Assert.Equal(category.Name, dto.Name);
    }

    [Fact]
    public void TagToTagResponse_MapsCorrectly()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<TagProfile>());

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = "Important",
            UserId = "user-1"
        };

        var dto = mapper.Map<TagResponse>(tag);

        Assert.Equal(tag.Id, dto.Id);
        Assert.Equal(tag.Name, dto.Name);
        Assert.Equal(tag.UserId, dto.UserId);
    }

    [Fact]
    public void TaskItemToTaskResponse_MapsCorrectly()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<TaskProfile>());

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Description = "Description",
            DueDate = DateTime.UtcNow.AddDays(1),
            Status = 2,
            CategoryId = Guid.NewGuid(),
            UserId = "user-1",
            SubTasks = new List<SubTask>
            {
                new() { Title = "Subtask 1", IsDone = true }
            },
            Tags = new List<Tag>
            {
                new() { Id = Guid.NewGuid(), Name = "Tag1", UserId = "user-1" }
            }
        };

        var dto = mapper.Map<TaskResponse>(task);

        Assert.Equal(task.Id, dto.Id);
        Assert.Equal(task.Title, dto.Title);
        Assert.Equal(task.Description, dto.Description);
        Assert.Equal(task.DueDate, dto.DueDate);
        Assert.Equal(task.Status, dto.Status);
        Assert.Equal(task.CategoryId, dto.CategoryId);
        Assert.Single(dto.SubTasks);
        Assert.Single(dto.Tags);
        Assert.Equal("Subtask 1", dto.SubTasks[0].Title);
        Assert.True(dto.SubTasks[0].IsDone);
        Assert.Equal("Tag1", dto.Tags[0].Name);
    }

    [Fact]
    public void SubTaskToSubTaskResponse_MapsCorrectly()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<TaskProfile>());

        var subTask = new SubTask
        {
            Title = "Test Subtask",
            IsDone = true
        };

        var dto = mapper.Map<SubTaskResponse>(subTask);

        Assert.Equal(subTask.Title, dto.Title);
        Assert.Equal(subTask.IsDone, dto.IsDone);
    }

    [Fact]
    public void TaskItemWithNoSubTasks_MapsToEmptyList()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<TaskProfile>());

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task",
            SubTasks = new List<SubTask>(),
            Tags = new List<Tag>()
        };

        var dto = mapper.Map<TaskResponse>(task);

        Assert.Empty(dto.SubTasks);
        Assert.Empty(dto.Tags);
    }

    [Fact]
    public void TaskItemWithNullDescription_MapsToNull()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<TaskProfile>());

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Task",
            Description = null,
            SubTasks = new List<SubTask>(),
            Tags = new List<Tag>()
        };

        var dto = mapper.Map<TaskResponse>(task);

        Assert.Null(dto.Description);
    }

    [Fact]
    public void CategoryWithSpecialCharacters_MapsCorrectly()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<CategoryProfile>());

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Work & Home / Projects (2024)"
        };

        var dto = mapper.Map<CategoryResponse>(category);

        Assert.Equal("Work & Home / Projects (2024)", dto.Name);
    }

    [Fact]
    public void TagWithEmptyUserId_MapsCorrectly()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<TagProfile>());

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = "System",
            UserId = ""
        };

        var dto = mapper.Map<TagResponse>(tag);

        Assert.Equal(tag.Id, dto.Id);
        Assert.Equal("System", dto.Name);
        Assert.Equal("", dto.UserId);
    }

    [Fact]
    public void TaskItem_WithAllNullOptionals_MapsCorrectly()
    {
        var mapper = CreateMapper(cfg => cfg.AddProfile<TaskProfile>());

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Minimal",
            Description = null,
            DueDate = null,
            Status = 1,
            CategoryId = null,
            SubTasks = new List<SubTask>(),
            Tags = new List<Tag>()
        };

        var dto = mapper.Map<TaskResponse>(task);

        Assert.Equal(task.Id, dto.Id);
        Assert.Equal("Minimal", dto.Title);
        Assert.Null(dto.Description);
        Assert.Null(dto.DueDate);
        Assert.Equal(1, dto.Status);
        Assert.Null(dto.CategoryId);
        Assert.Empty(dto.SubTasks);
        Assert.Empty(dto.Tags);
    }
}
