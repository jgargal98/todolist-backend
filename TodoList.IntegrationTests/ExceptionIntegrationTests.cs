namespace TodoList.IntegrationTests;

public sealed class ExceptionIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public ExceptionIntegrationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Returns500InternalServerError()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/exception");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
