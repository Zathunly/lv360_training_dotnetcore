using lv360_training.Application.Handlers;
using lv360_training.Domain.Entities;
using lv360_training.Domain.Dtos.Catalog.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lv360_training.Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehouseController : ControllerBase
{
    private readonly WarehouseHandler _warehouseHandler;

    public WarehouseController(WarehouseHandler warehouseHandler)
    {
        _warehouseHandler = warehouseHandler;
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var stocks = await _warehouseHandler.GetAllAsync();
        return Ok(stocks);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var warehouse = await _warehouseHandler.GetByIdAsync(id);
        if (warehouse == null) return NotFound();
        return Ok(warehouse);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateOrUpdateWarehouseDto dto)
    {
        var warehouse = new Warehouse
        {
            Name = dto.Name,
            Location = dto.Location,
        };

        var created = await _warehouseHandler.CreateAsync(warehouse);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateWarehouseDto dto)
    {
        if (dto == null) return BadRequest();

        var warehouse = new Warehouse
        {
            Name = dto.Name,
            Location = dto.Location
        };

        var updated = await _warehouseHandler.UpdateAsync(id, warehouse);
        if (updated == null) return NotFound();

        return Ok(updated);
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _warehouseHandler.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
