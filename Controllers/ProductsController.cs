using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService prodService)
    {
        _productService = prodService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        var prods = await _productService.GetAllProductsAsync();
        return Ok(prods);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var prod = await _productService.GetProductByIdAsync(id);
        if (prod == null)
            return NotFound(new { message = "Product not found" });

        return Ok(prod);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,InventoryManager")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest req)
    {
        try
        {
            var newProd = await _productService.CreateProductAsync(req);
            return CreatedAtAction(nameof(GetProduct), new { id = newProd.Id }, newProd);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating product: " + ex.Message);
            return StatusCode(500, new { message = "Error" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,InventoryManager")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest req)
    {
        try
        {
            var updated = await _productService.UpdateProductAsync(id, req);
            return Ok(updated);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating product: " + ex.Message);
            return StatusCode(500, new { message = "Update failed" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,InventoryManager")]
    public async Task<ActionResult> RemoveProduct(Guid id)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound(new { message = "Product not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error deleting product: " + ex.Message);
            return StatusCode(500, new { message = "Delete failed" });
        }
    }
}