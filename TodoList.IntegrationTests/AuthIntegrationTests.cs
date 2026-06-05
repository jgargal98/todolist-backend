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

    [Fact]
    public async Task Register_WithInvalidData_Returns400BadRequest()
    {
        var client = _factory.CreateClient();
        var request = new RegisterRequest("", "Pass123!", "Pass123!");

        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_Returns401Unauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("nonexistent@test.com", "Password123!"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_WithValidFlow_Returns200OkWithNewTokens()
    {
        var client = _factory.CreateClient();
        var email = $"refresh_{Guid.NewGuid():N}@test.com";
        var password = "Refresh@123";

        // Arrange: register to get initial tokens
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, password, password));
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        registerResult.Should().NotBeNull();

        // Act: refresh with the current tokens
        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest(registerResult!.AccessToken, registerResult.RefreshToken));

        // Assert
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        refreshResult.Should().NotBeNull();
        refreshResult!.AccessToken.Should().NotBeNullOrWhiteSpace();
        refreshResult.RefreshToken.Should().NotBeNullOrWhiteSpace();
        refreshResult.Email.Should().Be(email);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_Returns400BadRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest("invalid-access-token", "invalid-refresh-token"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_Returns400BadRequest()
    {
        var client = _factory.CreateClient();
        var email = $"weak_{Guid.NewGuid():N}@test.com";
        var request = new RegisterRequest(email, "weak", "weak");

        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithPasswordMismatch_Returns400BadRequest()
    {
        var client = _factory.CreateClient();
        var email = $"mismatch_{Guid.NewGuid():N}@test.com";
        var request = new RegisterRequest(email, "Valid@123", "Different@456");

        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWhitespaceEmail_Returns400BadRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest("   ", "Valid@123", "Valid@123"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_ReplayWithOldRefreshToken_Returns401Unauthorized()
    {
        var client = _factory.CreateClient();
        var email = $"replay_{Guid.NewGuid():N}@test.com";
        var password = "Replay@123";

        // Arrange: register to get initial tokens
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, password, password));
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var firstTokens = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        firstTokens.Should().NotBeNull();

        // Act 1: refresh once (this rotates the refresh token)
        var firstRefresh = await client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest(firstTokens!.AccessToken, firstTokens.RefreshToken));
        firstRefresh.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act 2: replay the ORIGINAL tokens (should fail because the refresh token was rotated)
        var replayResponse = await client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest(firstTokens.AccessToken, firstTokens.RefreshToken));

        // Assert
        replayResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
