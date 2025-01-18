using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewEra_Cash___Carry.Application.Interfaces.OrderInterfaces;
using NewEra_Cash___Carry.Core.DTOs.order;
using NewEra_Cash___Carry.Application.Interfaces.NotifyInterfaces;
using NewEra_Cash___Carry.DTOs.order.NewEra_Cash___Carry.DTOs.order;
using System.Security.Claims;

namespace NewEra_Cash___Carry.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;

        public OrderController(IOrderService orderService, INotificationService notificationService)
        {
            _orderService = orderService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                // Get the authenticated user's ID from the token
                var userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

                // Override the user ID in the order DTO to ensure it matches the authenticated user
                orderDto.UserId = userId;

                // Create the order
                var createdOrder = await _orderService.CreateOrderAsync(orderDto);

                // Send notification to the user
                var notificationMessage = $"Your order (ID: {createdOrder.Id}) has been created successfully.";
                await _notificationService.NotifyBySmsAsync(notificationMessage);

                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            try
            {
                // Update order status
                await _orderService.UpdateOrderStatusAsync(id, status);

                // Notify the user about the status update
                var order = await _orderService.GetOrderByIdAsync(id);
                var notificationMessage = $"Your order (ID: {order.Id}) status has been updated to '{status}'.";
                await _notificationService.NotifyBySmsAsync(notificationMessage);

                return Ok(new { message = "Order status updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                // Notify the user before deleting the order
                var order = await _orderService.GetOrderByIdAsync(id);
                var notificationMessage = $"Your order (ID: {order.Id}) has been canceled.";
                await _notificationService.NotifyBySmsAsync( notificationMessage);

                // Delete the order
                await _orderService.DeleteOrderAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
