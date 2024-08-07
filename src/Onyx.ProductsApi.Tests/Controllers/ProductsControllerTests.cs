using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Onyx.ProductsApi.Contracts;

namespace Onyx.ProductsApi.Tests.Controllers;

public class ProductsControllerTests : IDisposable
{
    private readonly string? _jwtToken = Environment.GetEnvironmentVariable("JWT_TOKEN");
    private readonly string? _basePath = Environment.GetEnvironmentVariable("API_PATH");
    private readonly HttpClient _client = new();
    private readonly string _path;

    public ProductsControllerTests()
    {
        _client.DefaultRequestHeaders.Authorization = new("Bearer", _jwtToken);
        _path = $"{_basePath}/api/products";
    }

    [Fact]
    public async Task GetProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var products = await CreateProducts();

        // Act
        var resp = await _client.GetFromJsonAsync<IEnumerable<Product>>(_path);

        // Assert
        resp.Should().Contain(products);
    }

    [Theory]
    [InlineData(Colours.Black)]
    [InlineData(Colours.Green)]
    [InlineData(Colours.Blue)]
    public async Task GetProducts_ShouldReturn_ProductsWithColour(Colours colour)
    {
        // Arrange
        await CreateProducts();

        // Act
        var resp = await _client.GetFromJsonAsync<IEnumerable<Product>>($"{_path}?Colours={colour}");

        // Assert
        resp.Should().AllSatisfy(x => x.Colour.Should().Be(colour));
    }

    [Fact]
    public async Task GetProducts_ShouldReturn400_IfInvalidColourIsSent()
    {
        // Arrange
        var colour = "INVALID_COLOUR";

        // Act
        var resp = await _client.GetAsync($"{_path}?Colours={colour}");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProducts_ShouldReturn401_NoAuthenticationIsProvided()
    {
        // Arrange
        var client = new HttpClient();

        // Act
        var resp = await client.GetAsync(_path);

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddProduct_ShouldCorrectlyAddAProduct()
    {
        // Arrange
        var product = new ProductCreate(Guid.NewGuid().ToString(), Colours.Green);

        // Act
        var response = await _client.PostAsJsonAsync(_path, product);
        var expected = await response.Content.ReadFromJsonAsync<Product>();

        // Assert
        var actual = await _client.GetFromJsonAsync<IEnumerable<Product>>($"{_path}?Colours={product.Colour}");

        actual.Should().Contain([expected!]);
    }

    [Fact]
    public async Task AddProduct_ShouldReturn401_NoAuthenticationIsProvided()
    {
        // Arrange
        var client = new HttpClient();

        // Act
        var resp = await client.PostAsJsonAsync(_path, new ProductCreate("Name", Colours.Black));

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<List<Product>> CreateProducts()
    {
        var result = new List<Product>();
        IEnumerable<ProductCreate> products = [
            new(Guid.NewGuid().ToString(), Colours.Black),
            new(Guid.NewGuid().ToString(), Colours.Blue),
            new(Guid.NewGuid().ToString(), Colours.Green)
        ];

        foreach (var product in products)
        {
            var resp = await _client.PostAsJsonAsync(_path, product);
            var item = await resp.Content.ReadFromJsonAsync<Product>();
            result.Add(item!);
        }

        return result;
    }

    public void Dispose()
        => GC.SuppressFinalize(this);
}
