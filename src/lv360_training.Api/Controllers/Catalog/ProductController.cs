using lv360_training.Application.Handlers;
using lv360_training.Domain.Dtos.Catalog.Request;
using lv360_training.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lv360_training.Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly ProductHandler _productHandler;

    public ProductController(ProductHandler productHandler)
    {
        _productHandler = productHandler;
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productHandler.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productHandler.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateOrUpdateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId
        };

        var created = await _productHandler.CreateAsync(product);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }


    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateProductDto dto)
    {
        var product = new Product
        {
            Id = id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId
        };

        var updated = await _productHandler.UpdateAsync(product);

        if (updated == null) return NotFound();

        return Ok(updated);
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productHandler.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateBulk([FromBody] IEnumerable<CreateOrUpdateProductDto> dtos)
    {
        var products = dtos.Select(dto => new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId
        }).ToList();

        var created = await _productHandler.CreateBulkAsync(products);
        return Ok(created);
    }

    [HttpPut("bulk")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBulk([FromBody] IEnumerable<UpdateProductBulkDto> dtos)
    {
        var products = dtos.Select(dto => new Product
        {
            Id = dto.Id, 
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        var updated = await _productHandler.UpdateBulkAsync(products);

        return Ok(updated);
    }

    [HttpDelete("bulk")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBulk([FromBody] IEnumerable<int> ids)
    {
        var count = await _productHandler.DeleteBulkAsync(ids);
        return Ok(new { deletedCount = count });
    }
}