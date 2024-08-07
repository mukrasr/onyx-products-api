using Onyx.ProductsApi.Contracts;

namespace Onyx.ProductsApi.Services;

public interface IProductsService
{
    Task AddProductAsync(Product product);
    Task<IEnumerable<Product>> GetProductsAsync(ProductQuery query);
    Task DeleteProductAsync(Colours colour);
}
