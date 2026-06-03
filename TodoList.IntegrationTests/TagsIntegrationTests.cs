namespace TodoList.IntegrationTests;

public sealed class TagsIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public TagsIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateTag_WithValidData_Returns201Created()
    {
        var client = _factory.CreateAuthenticatedClient();
        var request = new CreateTagRequest { Name = $"TestTag_{Guid.NewGuid():N}" };

        var response = await client.PostAsJsonAsync("/api/tags", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetAllTags_Returns200OkWithList()
    {
        var client = _factory.CreateAuthenticatedClient();
        var tagName = $"ListTag_{Guid.NewGuid():N}";

        // Arrange: create a tag first
        var createResponse = await client.PostAsJsonAsync("/api/tags",
            new CreateTagRequest { Name = tagName });
        createResponse.EnsureSuccessStatusCode();

        // Act
        var response = await client.GetAsync("/api/tags");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tags = await response.Content.ReadFromJsonAsync<List<TagResponse>>();
        tags.Should().NotBeNull();
        tags.Should().Contain(t => t.Name == tagName);
    }

    [Fact]
    public async Task AnyEndpoint_WithoutAuth_Returns401Unauthorized()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/tags");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
