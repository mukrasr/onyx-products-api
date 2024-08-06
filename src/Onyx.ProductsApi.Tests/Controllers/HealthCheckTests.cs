using System.Net;
using FluentAssertions;

namespace Onyx.ProductsApi.Tests.Controllers;

public class HealthCheckTests
{
    private readonly string _apiPath = Environment.GetEnvironmentVariable("API_PATH")!;

    [Fact]
    public async Task HealthCheckEndpoint_ShouldReturn200()
    {
        // Arrange
        var client = new HttpClient();

        // Act
        var resp = await client.GetAsync(_apiPath);

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

}
