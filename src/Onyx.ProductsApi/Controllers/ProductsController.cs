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
    public async Task<IActionResult> AddProductAsync(Product product)
    {
        var conflict = await service.GetProductsAsync(new([product.Colour]));

        if (conflict.Any())
        {
            return Conflict();
        }

        await service.AddProductAsync(product);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetProductsAsync([FromQuery] ProductQuery query)
        => Ok(await service.GetProductsAsync(query));

    [HttpDelete]
    [Route("{colour}")]
    public async Task<IActionResult> DeleteProductAsync(Colours colour)
    {
        var products = await service.GetProductsAsync(new([colour]));

        if (!products.Any())
        {
            return NotFound();
        }

        await service.DeleteProductAsync(colour);
        return Ok();
    }
}
