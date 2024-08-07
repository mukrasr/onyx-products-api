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
    private readonly IEnumerable<Product> _defaultProducts =
    [
        new(Colours.Black, Guid.NewGuid().ToString()),
        new(Colours.Blue, Guid.NewGuid().ToString()),
        new(Colours.Green, Guid.NewGuid().ToString())
    ];

    public ProductsControllerTests()
    {
        _client.DefaultRequestHeaders.Authorization = new("Bearer", _jwtToken);
        _path = $"{_basePath}/api/products";
    }

    [Fact]
    public async Task GetProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var products = await CreateDefaultProducts();

        // Act
        var resp = await _client.GetFromJsonAsync<IEnumerable<Product>>(_path);

        // Assert
        resp.Should().Contain(products);
        await DeleteProducts(products);
    }

    [Theory]
    [InlineData(Colours.Black)]
    [InlineData(Colours.Green)]
    [InlineData(Colours.Blue)]
    public async Task GetProducts_ShouldReturn_ProductsWithColour(Colours colour)
    {
        // Arrange
        var products = await CreateDefaultProducts();

        // Act
        var resp = await _client.GetFromJsonAsync<IEnumerable<Product>>($"{_path}?Colours={colour}");

        // Assert
        resp.Should().AllSatisfy(x => x.Colour.Should().Be(colour));
        await DeleteProducts(products);
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
        var product = new Product(Colours.Green, Guid.NewGuid().ToString());

        // Act
        await _client.PostAsJsonAsync(_path, product);

        // Assert
        var actual = await _client.GetFromJsonAsync<IEnumerable<Product>>($"{_path}?Colours={product.Colour}");

        actual.Should().Contain([product]);
        await DeleteProducts([product]);
    }

    [Fact]
    public async Task AddProduct_ShouldReturn409_WhenDuplicateProductExists()
    {
        // Arrange
        var product = new Product(Colours.Green, Guid.NewGuid().ToString());
        await _client.PostAsJsonAsync(_path, product);

        // Act
        var resp = await _client.PostAsJsonAsync(_path, new Product(Colours.Green, Guid.NewGuid().ToString()));

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
        await DeleteProducts([product]);
    }

    [Fact]
    public async Task AddProduct_ShouldReturn401_NoAuthenticationIsProvided()
    {
        // Arrange
        var client = new HttpClient();

        // Act
        var resp = await client.PostAsJsonAsync(_path, new Product(Colours.Black, "Name"));

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteProduct_ShouldCorrectlyDeleteAProduct()
    {
        // Arrange
        var product = new Product(Colours.Green, Guid.NewGuid().ToString());

        // Act
        await _client.DeleteAsync($"{_path}/{product.Colour}");

        // Assert
        var actual = await _client.GetFromJsonAsync<IEnumerable<Product>>($"{_path}?Colours={product.Colour}");

        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturn404_IfProductIsNotFound()
    {
        // Act
        var resp = await _client.DeleteAsync($"{_path}/{Colours.Black}");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturn401_NoAuthenticationIsProvided()
    {
        // Arrange
        var client = new HttpClient();

        // Act
        var resp = await client.DeleteAsync($"{_path}/{Colours.Black}");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<IEnumerable<Product>> CreateDefaultProducts()
    {
        foreach (var product in _defaultProducts)
        {
            await _client.PostAsJsonAsync(_path, product);
        }

        return _defaultProducts;
    }

    private async Task DeleteProducts(IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            await _client.DeleteAsync($"{_path}/{product.Colour}");
        }
    }

    public void Dispose()
        => GC.SuppressFinalize(this);
}
