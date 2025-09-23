using lv360_training.Application.Handlers;
using lv360_training.Domain.Entities;
using lv360_training.Application.Dtos.Catalog.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lv360_training.Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly StockHandler _stockHandler;

    public StockController(StockHandler stockHandler)
    {
        _stockHandler = stockHandler;
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var stocks = await _stockHandler.GetAllAsync();
        return Ok(stocks);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var stock = await _stockHandler.GetByIdAsync(id);
        if (stock == null) return NotFound();
        return Ok(stock);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateOrUpdateStockDto dto)
    {
        // Convert DTO to Stock entity
        var stock = new Stock
        {
            ProductId = dto.ProductId,
            WarehouseId = dto.WarehouseId,
            Quantity = dto.Quantity,
            LastUpdatedAt = DateTime.UtcNow
        };

        var created = await _stockHandler.CreateAsync(stock);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }


    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateOrUpdateStockDto dto)
    {
        var stockToUpdate = new Stock
        {
            ProductId = dto.ProductId,
            WarehouseId = dto.WarehouseId,
            Quantity = dto.Quantity,
            LastUpdatedAt = DateTime.UtcNow
        };

        var updated = await _stockHandler.UpdateAsync(id, stockToUpdate);
        if (updated == null) return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _stockHandler.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
