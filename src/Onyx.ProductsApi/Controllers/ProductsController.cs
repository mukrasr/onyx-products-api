using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onyx.ProductsApi.Contracts;
using Onyx.ProductsApi.Services;

namespace Onyx.ProductsApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductsService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddProductAsync(ProductCreate product)
        => Ok(await service.AddProductAsync(product));

    [HttpGet]
    public async Task<IActionResult> GetProductsAsync([FromQuery] ProductQuery query)
        => Ok(await service.GetProductsAsync(query));
}
