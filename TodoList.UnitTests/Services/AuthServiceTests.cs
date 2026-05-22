namespace TodoList.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _tokenProviderMock = new Mock<ITokenProvider>();
        _sut = new AuthService(_userRepoMock.Object, _tokenProviderMock.Object);
    }

    private static User CreateUser(string email = "test@example.com") => new()
    {
        Id = Guid.NewGuid().ToString(),
        Email = email,
        UserName = email
    };

    private static LoginRequest ValidLogin => new("test@example.com", "Password123!");

    private static RegisterRequest ValidRegister => new("new@example.com", "Password123!", "Password123!");

    private static RefreshRequest ValidRefresh(string token = "access-token", string refresh = "refresh-token")
        => new(token, refresh);

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        var user = CreateUser();
        _userRepoMock.Setup(r => r.ValidateCredentialsAsync(user.Email!, "Password123!"))
            .ReturnsAsync(user);
        _tokenProviderMock.Setup(t => t.Generate(user)).Returns("jwt-token");
        _tokenProviderMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");
        _userRepoMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var result = await _sut.LoginAsync(ValidLogin);

        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        _userRepoMock.Setup(r => r.ValidateCredentialsAsync("test@example.com", "Password123!"))
            .ReturnsAsync((User?)null);

        var result = await _sut.LoginAsync(ValidLogin);

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_WithUniqueEmail_CreatesUserAndReturnsAuthResponse()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync("new@example.com"))
            .ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.CreateAsync(It.IsAny<User>(), "Password123!"))
            .ReturnsAsync(true);
        _tokenProviderMock.Setup(t => t.Generate(It.IsAny<User>())).Returns("jwt-token");
        _tokenProviderMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

        var result = await _sut.RegisterAsync(ValidRegister);

        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsNull()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync("new@example.com"))
            .ReturnsAsync(CreateUser("new@example.com"));

        var result = await _sut.RegisterAsync(ValidRegister);

        Assert.Null(result);
        _userRepoMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenCreateFails_ThrowsException()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync("new@example.com"))
            .ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.CreateAsync(It.IsAny<User>(), "Password123!"))
            .ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() => _sut.RegisterAsync(ValidRegister));
        Assert.Contains("Database error", ex.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidTokens_ReturnsNewAuthResponse()
    {
        var user = CreateUser();
        user.RefreshToken = "valid-refresh";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, user.Email!)
        }));

        _tokenProviderMock.Setup(t => t.GetPrincipalFromExpiredToken("expired-token"))
            .Returns(principal);
        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email!)).ReturnsAsync(user);
        _tokenProviderMock.Setup(t => t.Generate(user)).Returns("new-jwt");
        _tokenProviderMock.Setup(t => t.GenerateRefreshToken()).Returns("new-refresh");
        _userRepoMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(true);

        var result = await _sut.RefreshTokenAsync(ValidRefresh("expired-token", "valid-refresh"));

        Assert.NotNull(result);
        Assert.Equal("new-jwt", result.AccessToken);
        Assert.Equal("new-refresh", result.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidPrincipal_ReturnsNull()
    {
        _tokenProviderMock.Setup(t => t.GetPrincipalFromExpiredToken("bad-token"))
            .Returns((ClaimsPrincipal?)null);

        var result = await _sut.RefreshTokenAsync(ValidRefresh("bad-token", "refresh"));

        Assert.Null(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenUserNotFound_ReturnsNull()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "ghost@example.com")
        }));

        _tokenProviderMock.Setup(t => t.GetPrincipalFromExpiredToken("expired-token"))
            .Returns(principal);
        _userRepoMock.Setup(r => r.GetByEmailAsync("ghost@example.com"))
            .ReturnsAsync((User?)null);

        var result = await _sut.RefreshTokenAsync(ValidRefresh("expired-token", "refresh"));

        Assert.Null(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithMismatchedRefreshToken_ReturnsNull()
    {
        var user = CreateUser();
        user.RefreshToken = "different-refresh";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, user.Email!)
        }));

        _tokenProviderMock.Setup(t => t.GetPrincipalFromExpiredToken("expired-token"))
            .Returns(principal);
        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email!)).ReturnsAsync(user);

        var result = await _sut.RefreshTokenAsync(ValidRefresh("expired-token", "wrong-refresh"));

        Assert.Null(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithExpiredRefreshToken_ReturnsNull()
    {
        var user = CreateUser();
        user.RefreshToken = "expired-refresh";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, user.Email!)
        }));

        _tokenProviderMock.Setup(t => t.GetPrincipalFromExpiredToken("expired-token"))
            .Returns(principal);
        _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email!)).ReturnsAsync(user);

        var result = await _sut.RefreshTokenAsync(ValidRefresh("expired-token", "expired-refresh"));

        Assert.Null(result);
    }

    [Fact]
    public async Task GenerateAuthResponse_WhenUpdateFails_ReturnsNull()
    {
        var user = CreateUser();
        _userRepoMock.Setup(r => r.ValidateCredentialsAsync(user.Email!, "Password123!"))
            .ReturnsAsync(user);
        _tokenProviderMock.Setup(t => t.Generate(user)).Returns("jwt");
        _tokenProviderMock.Setup(t => t.GenerateRefreshToken()).Returns("refresh");
        _userRepoMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(false);

        var result = await _sut.LoginAsync(new LoginRequest(user.Email!, "Password123!"));

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WhenTokenProviderThrows_PropagatesException()
    {
        var user = CreateUser();
        _userRepoMock.Setup(r => r.ValidateCredentialsAsync(user.Email!, "Password123!"))
            .ReturnsAsync(user);
        _tokenProviderMock.Setup(t => t.Generate(user)).Throws(new InvalidOperationException("RSA error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.LoginAsync(new LoginRequest(user.Email!, "Password123!")));
    }

    [Fact]
    public async Task RegisterAsync_WhenTokenProviderThrows_PropagatesException()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync("new@example.com"))
            .ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.CreateAsync(It.IsAny<User>(), "Password123!"))
            .ReturnsAsync(true);
        _tokenProviderMock.Setup(t => t.Generate(It.IsAny<User>()))
            .Throws(new InvalidOperationException("RSA error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RegisterAsync(ValidRegister));
    }
}
