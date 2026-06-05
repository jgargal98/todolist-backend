namespace TodoList.IntegrationTests;

public sealed class UsersIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public UsersIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUsers_Returns200OkWithUserList()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
        users.Should().NotBeNull();
        users.Should().Contain(u => u.Email == TestAuthHandler.TestEmail);
    }

    [Fact]
    public async Task GetUsers_WithoutAuth_Returns401Unauthorized()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsers_ReturnsAllRegisteredUsers_WithSeededAndNewUsers()
    {
        var client = _factory.CreateClient();
        var email = $"newuser_{Guid.NewGuid():N}@test.com";

        // Arrange: register a new user
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, "NewUser@123", "NewUser@123"));
        registerResponse.EnsureSuccessStatusCode();

        // Act: get all users as authenticated client
        var authClient = _factory.CreateAuthenticatedClient();
        var response = await authClient.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
        users.Should().NotBeNull();
        users.Should().Contain(u => u.Email == TestAuthHandler.TestEmail);
        users.Should().Contain(u => u.Email == email);
    }
}
