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
}
