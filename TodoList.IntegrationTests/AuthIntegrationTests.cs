namespace TodoList.IntegrationTests;

public sealed class AuthIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public AuthIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_WithValidData_Returns201CreatedWithTokens()
    {
        var client = _factory.CreateClient();
        var email = $"register_{Guid.NewGuid():N}@test.com";
        var request = new RegisterRequest(email, "Reg@1234", "Reg@1234");

        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
        authResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
        authResponse.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200OkWithTokens()
    {
        var client = _factory.CreateClient();
        var email = $"login_{Guid.NewGuid():N}@test.com";
        var password = "Login@123";

        // Arrange: register first
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, password, password));
        registerResponse.EnsureSuccessStatusCode();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(email, password));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
        authResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
        authResponse.Email.Should().Be(email);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409Conflict()
    {
        var client = _factory.CreateClient();
        var email = $"dup_{Guid.NewGuid():N}@test.com";
        var password = "Dup@1234";

        // Arrange: first registration
        var firstResponse = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, password, password));
        firstResponse.EnsureSuccessStatusCode();

        // Act: duplicate registration
        var response = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, password, password));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns401Unauthorized()
    {
        var client = _factory.CreateClient();
        var email = $"invalid_{Guid.NewGuid():N}@test.com";

        // Arrange: register
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, "Valid@123", "Valid@123"));
        registerResponse.EnsureSuccessStatusCode();

        // Act: login with wrong password
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(email, "WrongPassword1!"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
