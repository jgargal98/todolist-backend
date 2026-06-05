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

    [Fact]
    public async Task DeleteTag_WithExistingId_Returns204NoContent()
    {
        var client = _factory.CreateAuthenticatedClient();
        var tagName = $"DeleteTag_{Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("/api/tags",
            new CreateTagRequest { Name = tagName });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<TagResponse>();

        var deleteResponse = await client.DeleteAsync($"/api/tags/{created!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateTag_WithEmptyName_Returns400BadRequest()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/tags",
            new CreateTagRequest { Name = "" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteTag_WithNonExistent_Returns404NotFound()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.DeleteAsync($"/api/tags/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllTags_WhenEmpty_Returns200OkWithEmptyList()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/tags");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tags = await response.Content.ReadFromJsonAsync<List<TagResponse>>();
        tags.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTag_WithNameExceedingMaxLength_Returns400BadRequest()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/tags",
            new CreateTagRequest { Name = new string('a', 51) });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTag_WithWhitespaceName_Returns400BadRequest()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/tags",
            new CreateTagRequest { Name = "   " });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllTags_AfterDelete_DoesNotReturnDeletedTag()
    {
        var client = _factory.CreateAuthenticatedClient();
        var tagName = $"GoneTag_{Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("/api/tags",
            new CreateTagRequest { Name = tagName });
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<TagResponse>();

        await client.DeleteAsync($"/api/tags/{created!.Id}");

        var response = await client.GetAsync("/api/tags");
        var tags = await response.Content.ReadFromJsonAsync<List<TagResponse>>();

        tags.Should().NotContain(t => t.Id == created.Id);
    }
}
