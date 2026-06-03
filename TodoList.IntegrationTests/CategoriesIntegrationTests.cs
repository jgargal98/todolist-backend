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
}
