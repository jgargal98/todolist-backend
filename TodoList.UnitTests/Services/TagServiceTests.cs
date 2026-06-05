namespace TodoList.UnitTests.Services;

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _tagRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TagService _sut;

    public TagServiceTests()
    {
        _tagRepoMock = new Mock<ITagRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();

        _mapperMock.Setup(m => m.Map<TagResponse>(It.IsAny<Tag>()))
            .Returns((Tag t) => new TagResponse { Id = t.Id, Name = t.Name, UserId = t.UserId });

        _mapperMock.Setup(m => m.Map<IEnumerable<TagResponse>>(It.IsAny<IEnumerable<Tag>>()))
            .Returns((IEnumerable<Tag> tags) => tags?.Select(t => new TagResponse { Id = t.Id, Name = t.Name, UserId = t.UserId }).ToList() ?? new List<TagResponse>());

        _sut = new TagService(_tagRepoMock.Object, _userRepoMock.Object, _mapperMock.Object);
    }

    private static string UserId => "user-1";

    private static List<User> UsersWith(string userId) =>
        [new() { Id = userId, Email = "u@test.com", UserName = "u" }];

    [Fact]
    public async Task GetUserTagsAsync_ReturnsMappedTags()
    {
        var tags = new List<Tag>
        {
            new() { Id = Guid.NewGuid(), Name = "Tag1", UserId = UserId },
            new() { Id = Guid.NewGuid(), Name = "Tag2", UserId = UserId }
        };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(tags);

        var result = await _sut.GetUserTagsAsync(UserId);

        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.Name == "Tag1");
    }

    [Fact]
    public async Task GetUserTagsAsync_WhenUserNotFound_ReturnsEmpty()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<User>());

        var result = await _sut.GetUserTagsAsync("unknown");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserTagsAsync_WhenUsersNull_ReturnsEmpty()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync((IEnumerable<User>)null!);

        var result = await _sut.GetUserTagsAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserTagsAsync_WithNoTags_ReturnsEmpty()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(Enumerable.Empty<Tag>());

        var result = await _sut.GetUserTagsAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateTagAsync_WithValidData_ReturnsTagResponse()
    {
        var request = new CreateTagRequest { Name = "  New Tag  " };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.AddAsync(It.IsAny<Tag>()))
            .ReturnsAsync(true);

        var result = await _sut.CreateTagAsync(UserId, request);

        Assert.NotNull(result);
        Assert.Equal("New Tag", result.Name);
        Assert.Equal(UserId, result.UserId);
    }

    [Fact]
    public async Task CreateTagAsync_WhenUserNotFound_ReturnsNull()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<User>());

        var result = await _sut.CreateTagAsync("unknown", new CreateTagRequest { Name = "Tag" });

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTagAsync_WhenAddFails_ThrowsException()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.AddAsync(It.IsAny<Tag>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _sut.CreateTagAsync(UserId, new CreateTagRequest { Name = "Tag" }));
        Assert.Contains("Database error", ex.Message);
    }

    [Fact]
    public async Task DeleteTagAsync_WithExistingTag_ReturnsTrue()
    {
        var tagId = Guid.NewGuid();
        var tag = new Tag { Id = tagId, Name = "Tag", UserId = UserId };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.GetByIdAndUserIdAsync(tagId, UserId))
            .ReturnsAsync(tag);
        _tagRepoMock.Setup(r => r.DeleteAsync(tagId))
            .ReturnsAsync(true);

        var result = await _sut.DeleteTagAsync(tagId, UserId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTagAsync_WhenUserNotFound_ReturnsFalse()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<User>());

        var result = await _sut.DeleteTagAsync(Guid.NewGuid(), "unknown");

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTagAsync_WhenTagNotFound_ReturnsFalse()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.GetByIdAndUserIdAsync(It.IsAny<Guid>(), UserId))
            .ReturnsAsync((Tag?)null);

        var result = await _sut.DeleteTagAsync(Guid.NewGuid(), UserId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTagAsync_WhenTagBelongsToOtherUser_ReturnsFalse()
    {
        var tagId = Guid.NewGuid();
        var tag = new Tag { Id = tagId, Name = "Other's Tag", UserId = "other-user" };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.GetByIdAndUserIdAsync(tagId, UserId))
            .ReturnsAsync((Tag?)null);

        var result = await _sut.DeleteTagAsync(tagId, UserId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTagAsync_WhenDeleteFails_ReturnsFalse()
    {
        var tagId = Guid.NewGuid();
        var tag = new Tag { Id = tagId, Name = "Tag", UserId = UserId };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.GetByIdAndUserIdAsync(tagId, UserId))
            .ReturnsAsync(tag);
        _tagRepoMock.Setup(r => r.DeleteAsync(tagId))
            .ReturnsAsync(false);

        var result = await _sut.DeleteTagAsync(tagId, UserId);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateTagAsync_TrimsWhitespaceName()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.AddAsync(It.IsAny<Tag>()))
            .ReturnsAsync(true);

        Tag? captured = null;
        _tagRepoMock.Setup(r => r.AddAsync(It.IsAny<Tag>()))
            .Callback<Tag>(t => captured = t)
            .ReturnsAsync(true);

        var result = await _sut.CreateTagAsync(UserId, new CreateTagRequest { Name = "  Important  " });

        Assert.NotNull(result);
        Assert.Equal("Important", captured!.Name);
    }

    [Fact]
    public async Task CreateTagAsync_WhenRepositoryThrows_PropagatesException()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.AddAsync(It.IsAny<Tag>()))
            .Throws(new InvalidOperationException("DB insert failed"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateTagAsync(UserId, new CreateTagRequest { Name = "Tag" }));
    }

    [Fact]
    public async Task DeleteTagAsync_WhenRepositoryThrows_PropagatesException()
    {
        var tagId = Guid.NewGuid();
        var tag = new Tag { Id = tagId, Name = "Tag", UserId = UserId };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _tagRepoMock.Setup(r => r.GetByIdAndUserIdAsync(tagId, UserId))
            .ReturnsAsync(tag);
        _tagRepoMock.Setup(r => r.DeleteAsync(tagId))
            .Throws(new InvalidOperationException("DB error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.DeleteTagAsync(tagId, UserId));
    }
}
