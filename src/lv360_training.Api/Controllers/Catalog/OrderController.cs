using lv360_training.Application.Handlers;
using lv360_training.Domain.Entities;
using lv360_training.Application.Dtos.Catalog.Request;
using lv360_training.Application.Dtos.Catalog.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using System.Security.Claims;

namespace lv360_training.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly OrderHandler _orderHandler;

    public OrderController(OrderHandler orderHandler)
    {
        _orderHandler = orderHandler;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<IEnumerable<Order>>> GetAll()
    {
        var orders = await _orderHandler.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<Order>> GetById(int id)
    {
        var order = await _orderHandler.GetByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }


    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequestDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Map DTO -> Order entity
        var order = new Order
        {
            UserId = userId,
            OrderItems = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        // Place order via handler (handles validation, unit price, stock, etc.)
        var createdOrder = await _orderHandler.PlaceOrder(order);

        // Inline mapping to DTO for response
        var resultDto = new OrderSuccessDto
        {
            Id = createdOrder.Id,
            OrderDate = createdOrder.OrderDate,
            Status = createdOrder.Status,
            TotalAmount = createdOrder.TotalAmount,
            Items = createdOrder.OrderItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product.Name, 
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, resultDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderRequestDto dto)
    {
        try
        {
            var order = await _orderHandler.GetByIdAsync(id);
            if (order == null) return NotFound();

            // Update order status
            order.Status = dto.Status;

            // Update order items
            order.OrderItems = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = order.OrderItems
                    .FirstOrDefault(oi => oi.ProductId == i.ProductId)?.UnitPrice ?? 0m
            }).ToList();

            var updatedOrder = await _orderHandler.UpdateAsync(order);

            // Inline mapping to DTO for response
            var resultDto = new OrderSuccessDto
            {
                Id = updatedOrder.Id,
                OrderDate = updatedOrder.OrderDate,
                Status = updatedOrder.Status,
                TotalAmount = updatedOrder.TotalAmount,
                Items = updatedOrder.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name, 
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            return Ok(resultDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _orderHandler.Delete(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
