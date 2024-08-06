using Onyx.ProductsApi.Contracts;

namespace Onyx.ProductsApi.Services;

public interface IProductsService
{
    Task<Product> AddProductAsync(ProductCreate product);
    Task<IEnumerable<Product>> GetProductsAsync(ProductQuery query);
}
