using Onyx.ProductsApi.Contracts;

namespace Onyx.ProductsApi.Services;

public class ProductsService : IProductsService
{
    private static readonly HashSet<Product> Products = [];

    public Task<Product> AddProductAsync(ProductCreate product)
    {
        var item = new Product(Products.Count + 1, product.Name, product.Colour);

        if (!Products.Add(item))
        {
            throw new Exception($"Could not add item: {product}");
        }

        return Task.FromResult(item);
    }

    public Task<IEnumerable<Product>> GetProductsAsync(ProductQuery query)
    {
        var result = Products.AsEnumerable();

        if (query.Colours != null)
        {
            result = result.Where(x => query.Colours.Contains(x.Colour));
        }

        return Task.FromResult(result);
    }
}
