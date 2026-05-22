namespace TodoList.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();

        _mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDto>>(It.IsAny<IEnumerable<User>>()))
            .Returns((IEnumerable<User> users) => users?.Select(u => new UserResponseDto
            {
                Id = Guid.Parse(u.Id),
                Email = u.Email!,
                UserName = u.UserName!
            }).ToList() ?? new List<UserResponseDto>());

        _sut = new UserService(_userRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsMappedUsers()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid().ToString(), Email = "user1@test.com", UserName = "user1" },
            new() { Id = Guid.NewGuid().ToString(), Email = "user2@test.com", UserName = "user2" }
        };

        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(users);

        var result = await _sut.GetUsersAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.Email == "user1@test.com");
    }

    [Fact]
    public async Task GetUsersAsync_WithNoUsers_ReturnsEmpty()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<User>());

        var result = await _sut.GetUsersAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUsersAsync_WhenRepoReturnsNull_DoesNotThrow()
    {
        _userRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync((IEnumerable<User>)null!);

        var result = await _sut.GetUsersAsync();

        Assert.Empty(result);
    }
}
