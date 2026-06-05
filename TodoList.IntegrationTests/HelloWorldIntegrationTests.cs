namespace TodoList.IntegrationTests;

public sealed class HelloWorldIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public HelloWorldIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Returns200Ok()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/helloworld");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var message = await response.Content.ReadAsStringAsync();
        message.Should().Be("\"hello from the api\"");
    }
}
