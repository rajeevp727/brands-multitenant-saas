using GreenPantry.Application.DTOs.Order;
using GreenPantry.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenPantry.API.Controllers;

[Authorize]
public class OrdersController : BaseApiController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderRequest request)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized();
        }

        var order = await _orderService.CreateOrderAsync(request, CurrentUserId);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(string id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        if (order.UserId != CurrentUserId && !IsAdmin && !IsUserInRole("Vendor"))
        {
            return Forbid();
        }

        return Ok(order);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUser(string userId)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        return Ok(orders);
    }

    [HttpGet("restaurant/{restaurantId}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByRestaurant(string restaurantId)
    {
        var orders = await _orderService.GetOrdersByRestaurantIdAsync(restaurantId);
        return Ok(orders);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Vendor,Admin,Delivery")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(string id, UpdateOrderStatusRequest request)
    {
        var order = await _orderService.UpdateOrderStatusAsync(id, request);
        return Ok(order);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelOrder(string id)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized();
        }

        var success = await _orderService.CancelOrderAsync(id, CurrentUserId);
        if (!success)
        {
            return BadRequest(new { message = "Unable to cancel order" });
        }

        return Ok(new { message = "Order cancelled successfully" });
    }
}
