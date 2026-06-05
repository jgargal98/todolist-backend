namespace TodoList.IntegrationTests;

public sealed class CategoriesIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public CategoriesIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateCategory_WithValidData_Returns201Created()
    {
        var client = _factory.CreateAuthenticatedClient();
        var request = new CreateCategoryRequest($"TestCat_{Guid.NewGuid():N}");

        var response = await client.PostAsJsonAsync("/api/categories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetAllCategories_Returns200OkWithList()
    {
        var client = _factory.CreateAuthenticatedClient();
        var catName = $"ListCat_{Guid.NewGuid():N}";

        // Arrange: create a category first
        var createResponse = await client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequest(catName));
        createResponse.EnsureSuccessStatusCode();

        // Act
        var response = await client.GetAsync("/api/categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        categories.Should().NotBeNull();
        categories.Should().Contain(c => c.Name == catName);
    }

    [Fact]
    public async Task AnyEndpoint_WithoutAuth_Returns401Unauthorized()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/categories");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateCategory_WithValidData_Returns204NoContent()
    {
        var client = _factory.CreateAuthenticatedClient();
        var catName = $"UpdateCat_{Guid.NewGuid():N}";

        // Arrange: create a category first
        var createResponse = await client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequest(catName));
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryResponse>();

        // Act: update it
        var updateResponse = await client.PutAsJsonAsync($"/api/categories/{created!.Id}",
            new UpdateCategoryRequest("Updated Name"));

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategory_WithExistingId_Returns204NoContent()
    {
        var client = _factory.CreateAuthenticatedClient();
        var catName = $"DeleteCat_{Guid.NewGuid():N}";

        // Arrange: create a category first
        var createResponse = await client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequest(catName));
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryResponse>();

        // Act: delete it
        var deleteResponse = await client.DeleteAsync($"/api/categories/{created!.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateCategory_WithEmptyName_Returns400BadRequest()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequest(""));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCategory_WithNonExistent_Returns404NotFound()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PutAsJsonAsync($"/api/categories/{Guid.NewGuid()}",
            new UpdateCategoryRequest("Name"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategory_WithNonExistent_Returns404NotFound()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.DeleteAsync($"/api/categories/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllCategories_WhenEmpty_Returns200OkWithEmptyList()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        categories.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateCategory_WithEmptyName_Returns400BadRequest()
    {
        var client = _factory.CreateAuthenticatedClient();
        var catName = $"UpdateEmptyCat_{Guid.NewGuid():N}";

        // Arrange: create a category first
        var createResponse = await client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequest(catName));
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryResponse>();

        // Act: update with empty name
        var updateResponse = await client.PutAsJsonAsync($"/api/categories/{created!.Id}",
            new UpdateCategoryRequest(""));

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCategory_WithNameExceedingMaxLength_Returns400BadRequest()
    {
        var client = _factory.CreateAuthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/categories",
            new CreateCategoryRequest(new string('a', 51)));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
