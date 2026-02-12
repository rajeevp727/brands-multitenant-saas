using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vendor.API.Extensions;
using Vendor.Application.DTOs.Order;
using Vendor.Application.Interfaces;

namespace Vendor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Vendor")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var vendorId = User.GetVendorId();
        var orders = await _orderService.GetVendorOrdersAsync(vendorId);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var vendorId = User.GetVendorId();
        var order = await _orderService.GetOrderByIdAsync(id, vendorId);
        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(order);
    }

    [HttpPost("{id}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptOrder(int id)
    {
        var vendorId = User.GetVendorId();
        var accepted = await _orderService.AcceptOrderAsync(id, vendorId);
        if (!accepted)
            return BadRequest(new { message = "Order cannot be accepted" });

        return Ok(new { message = "Order accepted" });
    }

    [HttpPost("{id}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectOrder(int id)
    {
        var vendorId = User.GetVendorId();
        var rejected = await _orderService.RejectOrderAsync(id, vendorId);
        if (!rejected)
            return BadRequest(new { message = "Order cannot be rejected" });

        return Ok(new { message = "Order rejected" });
    }

    [HttpPost("{id}/prepare")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkPreparing(int id)
    {
        var vendorId = User.GetVendorId();
        var updated = await _orderService.MarkPreparingAsync(id, vendorId);
        if (!updated)
            return BadRequest(new { message = "Order cannot be marked as preparing" });

        return Ok(new { message = "Order marked as preparing" });
    }

    [HttpPost("{id}/dispatch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DispatchOrder(int id)
    {
        var vendorId = User.GetVendorId();
        var dispatched = await _orderService.DispatchOrderAsync(id, vendorId);
        if (!dispatched)
            return BadRequest(new { message = "Order cannot be dispatched" });

        return Ok(new { message = "Order dispatched" });
    }

    [HttpPost("{id}/deliver")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkDelivered(int id)
    {
        var vendorId = User.GetVendorId();
        var delivered = await _orderService.MarkDeliveredAsync(id, vendorId);
        if (!delivered)
            return BadRequest(new { message = "Order cannot be marked as delivered" });

        return Ok(new { message = "Order delivered" });
    }
}

