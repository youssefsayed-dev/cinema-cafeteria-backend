using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Cashier,Admin")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;
    private readonly System.Security.Claims.ClaimsPrincipal _currentUser;

    public SalesController(ISaleService saleService, IHttpContextAccessor httpContext)
    {
        _saleService = saleService;
        _currentUser = httpContext.HttpContext?.User;
    }

    private Guid GetUserId()
    {
        var userIdStr = _currentUser.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
            throw new UnauthorizedAccessException("Token not valid");

        return userId;
    }

    [HttpPost]
    public async Task<ActionResult<SaleDto>> CreateSale([FromBody] CreateSaleRequest req)
    {
        try
        {
            var sale = await _saleService.CreateSaleAsync(req);
            return CreatedAtAction(nameof(GetSales), new { id = sale.Id }, sale);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating sale: " + ex.Message);
            return StatusCode(500, new { message = "Sale creation failed" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<SaleDto>>> GetSales()
    {
        var allSales = await _saleService.GetAllSalesAsync();
        return Ok(allSales);
    }
}