using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.DTOs.order;
using NewEra_Cash___Carry.DTOs.order.NewEra_Cash___Carry.DTOs.order;
using NewEra_Cash___Carry.Models;

namespace NewEra_Cash___Carry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;

        public OrderController(RetailOrderingSystemDbContext context)
        {
            _context = context;
        }

        // Get all orders (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                UserName = o.User.PhoneNumber, // Adjust based on your User model
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        // Get an order by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User.PhoneNumber,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            return Ok(orderDto);
        }

        // Create an order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            var user = await _context.Users.FindAsync(orderDto.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid user ID." });
            }

            var order = new Order
            {
                UserId = orderDto.UserId,
                TotalAmount = 0,
                Status = "Pending",
                OrderItems = new List<OrderItem>()
            };

            foreach (var itemDto in orderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                {
                    return BadRequest(new { message = $"Product with ID {itemDto.ProductId} not found." });
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.ProductId,
                    Quantity = itemDto.Quantity,
                    Price = product.Price
                };

                order.OrderItems.Add(orderItem);
                order.TotalAmount += product.Price * itemDto.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Transform to DTO to avoid cycles
            var orderDtoResponse = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = user.PhoneNumber,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, orderDtoResponse);
        }


        // Update order status (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order status updated successfully." });
        }

        // Delete an order (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
