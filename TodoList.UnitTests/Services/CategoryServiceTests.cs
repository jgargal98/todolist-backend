namespace TodoList.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _catRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _catRepoMock = new Mock<ICategoryRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();

        _mapperMock.Setup(m => m.Map<CategoryResponse>(It.IsAny<Category>()))
            .Returns((Category c) => new CategoryResponse { Id = c.Id, Name = c.Name });

        _mapperMock.Setup(m => m.Map<IEnumerable<CategoryResponse>>(It.IsAny<IEnumerable<Category>>()))
            .Returns((IEnumerable<Category> cats) => cats?.Select(c => new CategoryResponse { Id = c.Id, Name = c.Name }).ToList() ?? new List<CategoryResponse>());

        _sut = new CategoryService(_catRepoMock.Object, _userRepoMock.Object, _mapperMock.Object);
    }

    private static string UserId => "user-1";

    private static List<User> UsersWith(string userId) =>
        [new() { Id = userId, Email = "u@test.com", UserName = "u" }];

    [Fact]
    public async Task GetUserCategoriesAsync_ReturnsMappedCategories()
    {
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Work", UserId = UserId },
            new() { Id = Guid.NewGuid(), Name = "Personal", UserId = UserId }
        };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(categories);

        var result = await _sut.GetUserCategoriesAsync(UserId);

        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Name == "Work");
    }

    [Fact]
    public async Task GetUserCategoriesAsync_WhenUserNotFound_ReturnsEmpty()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<User>());

        var result = await _sut.GetUserCategoriesAsync("unknown");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserCategoriesAsync_WhenUsersNull_ReturnsEmpty()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync((IEnumerable<User>)null!);

        var result = await _sut.GetUserCategoriesAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserCategoriesAsync_WithNoCategories_ReturnsEmpty()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.GetByUserIdAsync(UserId))
            .ReturnsAsync(Enumerable.Empty<Category>());

        var result = await _sut.GetUserCategoriesAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithValidData_ReturnsCategoryResponse()
    {
        var request = new CreateCategoryRequest("  New Category  ");

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(true);

        var result = await _sut.CreateCategoryAsync(UserId, request);

        Assert.NotNull(result);
        Assert.Equal("New Category", result.Name);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenUserNotFound_ReturnsNull()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<User>());

        var result = await _sut.CreateCategoryAsync("unknown", new CreateCategoryRequest("Name"));

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenAddFails_ThrowsException()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _sut.CreateCategoryAsync(UserId, new CreateCategoryRequest("Name")));
        Assert.Contains("Database error", ex.Message);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithValidData_ReturnsTrue()
    {
        var catId = Guid.NewGuid();
        var category = new Category { Id = catId, Name = "Old", UserId = UserId };
        var request = new UpdateCategoryRequest("Updated");

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.GetByIdAndUserIdAsync(catId, UserId))
            .ReturnsAsync(category);
        _catRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(true);

        var result = await _sut.UpdateCategoryAsync(catId, UserId, request);

        Assert.True(result);
        Assert.Equal("Updated", category.Name);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WhenUserNotFound_ReturnsFalse()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<User>());

        var result = await _sut.UpdateCategoryAsync(Guid.NewGuid(), "unknown",
            new UpdateCategoryRequest("Name"));

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WhenCategoryNotFound_ReturnsFalse()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.GetByIdAndUserIdAsync(It.IsAny<Guid>(), UserId))
            .ReturnsAsync((Category?)null);

        var result = await _sut.UpdateCategoryAsync(Guid.NewGuid(), UserId,
            new UpdateCategoryRequest("Name"));

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WhenCategoryBelongsToOtherUser_ReturnsFalse()
    {
        var catId = Guid.NewGuid();
        var category = new Category { Id = catId, Name = "Other's", UserId = "other-user" };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.GetByIdAndUserIdAsync(catId, UserId))
            .ReturnsAsync((Category?)null);

        var result = await _sut.UpdateCategoryAsync(catId, UserId,
            new UpdateCategoryRequest("Name"));

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WhenUpdateFails_ThrowsException()
    {
        var catId = Guid.NewGuid();
        var category = new Category { Id = catId, Name = "Old", UserId = UserId };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.GetByIdAndUserIdAsync(catId, UserId))
            .ReturnsAsync(category);
        _catRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _sut.UpdateCategoryAsync(catId, UserId, new UpdateCategoryRequest("New")));
        Assert.Contains("Database error", ex.Message);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithExistingCategory_ReturnsTrue()
    {
        var catId = Guid.NewGuid();
        var category = new Category { Id = catId, Name = "Category", UserId = UserId };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.GetByIdAndUserIdAsync(catId, UserId))
            .ReturnsAsync(category);
        _catRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Category>()))
            .ReturnsAsync(true);

        var result = await _sut.DeleteCategoryAsync(catId, UserId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenUserNotFound_ReturnsFalse()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<User>());

        var result = await _sut.DeleteCategoryAsync(Guid.NewGuid(), "unknown");

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenCategoryNotFound_ReturnsFalse()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.GetByIdAndUserIdAsync(It.IsAny<Guid>(), UserId))
            .ReturnsAsync((Category?)null);

        var result = await _sut.DeleteCategoryAsync(Guid.NewGuid(), UserId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenDeleteFails_ThrowsException()
    {
        var catId = Guid.NewGuid();
        var category = new Category { Id = catId, Name = "Category", UserId = UserId };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(UsersWith(UserId));
        _catRepoMock.Setup(r => r.GetByIdAndUserIdAsync(catId, UserId))
            .ReturnsAsync(category);
        _catRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Category>()))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _sut.DeleteCategoryAsync(catId, UserId));
        Assert.Contains("Database error", ex.Message);
    }
}
