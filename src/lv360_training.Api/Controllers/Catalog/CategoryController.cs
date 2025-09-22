using lv360_training.Application.Handlers;
using lv360_training.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lv360_training.Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class CategoryController : ControllerBase
{
    private readonly CategoryHandler _categoryHandler;

    public CategoryController(CategoryHandler categoryHandler)
    {
        _categoryHandler = categoryHandler;
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryHandler.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryHandler.GetByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Category category)
    {
        var created = await _categoryHandler.CreateAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.Id) return BadRequest("ID mismatch");

        var updated = await _categoryHandler.UpdateAsync(category);
        if (updated == null) return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _categoryHandler.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
