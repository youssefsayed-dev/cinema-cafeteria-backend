using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "InventoryManager")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventory;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventory = inventoryService;
    }

    [HttpPost("add")]
    public async Task<ActionResult<InventoryBatchDto>> AddInventoryBatch([FromBody] AddInventoryBatchRequest req)
    {
        try
        {
            var newBatch = await _inventory.AddInventoryBatchAsync(req);
            return CreatedAtAction(nameof(AddInventoryBatch), new { id = newBatch.Id }, newBatch);
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
            Console.WriteLine("Error adding batch: " + ex.Message);
            return StatusCode(500, new { message = "Error adding inventory" });
        }
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<List<LowStockProductDto>>> GetLowStockProducts([FromQuery] int threshold = 10)
    {
        try
        {
            var lowStockItems = await _inventory.GetLowStockProductsAsync(threshold);
            return Ok(lowStockItems);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting low stock: " + ex.Message);
            return StatusCode(500, new { message = "Error getting low stock items" });
        }
    }
}