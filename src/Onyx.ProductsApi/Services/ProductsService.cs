using Onyx.ProductsApi.Contracts;

namespace Onyx.ProductsApi.Services;

public class ProductsService(ILogger<ProductsService> logger) : IProductsService
{
    private static readonly Dictionary<Colours, Product> Products = [];

    public Task AddProductAsync(Product product)
    {
        if (Products.ContainsKey(product.Colour))
        {
            logger.LogError("Product already exists in system: {Product}", product);
            return Task.CompletedTask;
        }

        Products.Add(product.Colour, product);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Product>> GetProductsAsync(ProductQuery query)
    {
        var result = Products.AsEnumerable();

        if (query.Colours != null)
        {
            result = result.Where(x => query.Colours.Contains(x.Key));
        }

        return Task.FromResult(result.Select(x => x.Value));
    }

    public Task DeleteProductAsync(Colours colour)
    {
        if (!Products.ContainsKey(colour))
        {
            logger.LogError("Product with {Colour} not found", colour);
            return Task.CompletedTask;
        }

        if (!Products.Remove(colour))
        {
            throw new Exception($"Could not remove product with colour: {colour}");
        }

        return Task.CompletedTask;
    }
}
