using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.DTOs.order;
using NewEra_Cash___Carry.DTOs.order.NewEra_Cash___Carry.DTOs.order;
using NewEra_Cash___Carry.Models;
using Serilog;

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
            try
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
                    UserName = o.User.PhoneNumber,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    PaymentIntentId = o.PaymentIntentId,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                }).ToList();

                Log.Information("Fetched all orders successfully.");
                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching orders.");
                throw;
            }
        }

        // Get an order by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    Log.Warning("Order with ID {OrderId} not found.", id);
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
                    PaymentStatus = order.PaymentStatus,
                    PaymentIntentId = order.PaymentIntentId,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                };

                Log.Information("Order with ID {OrderId} retrieved successfully.", id);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving order with ID {OrderId}.", id);
                throw;
            }
        }

        // Create an order
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            if (orderDto == null || !orderDto.OrderItems.Any())
            {
                Log.Warning("Invalid order creation attempt with empty body or items.");
                return BadRequest(new { message = "Order details cannot be empty." });
            }

            try
            {
                var user = await _context.Users.FindAsync(orderDto.UserId);
                if (user == null)
                {
                    Log.Warning("Invalid user ID {UserId} provided for order creation.", orderDto.UserId);
                    return BadRequest(new { message = "Invalid user ID." });
                }

                var order = new Order
                {
                    UserId = orderDto.UserId,
                    TotalAmount = 0,
                    Status = "Pending",
                    PaymentStatus = "Pending",
                    OrderItems = new List<OrderItem>()
                };

                foreach (var itemDto in orderDto.OrderItems)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null)
                    {
                        Log.Warning("Product with ID {ProductId} not found.", itemDto.ProductId);
                        return BadRequest(new { message = $"Product with ID {itemDto.ProductId} not found." });
                    }

                    if (product.Stock < itemDto.Quantity)
                    {
                        Log.Warning("Insufficient stock for product {ProductName}. Available: {Stock}, Requested: {Requested}.", product.Name, product.Stock, itemDto.Quantity);
                        return BadRequest(new { message = $"Not enough stock for product {product.Name}. Available stock: {product.Stock}" });
                    }

                    if (itemDto.Quantity <= 0)
                    {
                        Log.Warning("Invalid quantity {Quantity} for product ID {ProductId}.", itemDto.Quantity, product.ProductId);
                        return BadRequest(new { message = $"Quantity must be greater than 0 for product {product.Name}." });
                    }

                    product.Stock -= itemDto.Quantity;

                    var orderItem = new OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = itemDto.Quantity,
                        Price = product.Price
                    };

                    order.OrderItems.Add(orderItem);
                    order.TotalAmount += product.Price * itemDto.Quantity;
                }

                if (order.TotalAmount <= 0)
                {
                    Log.Warning("Attempt to create an order with a total amount of 0 for user ID {UserId}.", order.UserId);
                    return BadRequest(new { message = "Order total amount must be greater than 0." });
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var orderDtoResponse = new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    UserName = user.PhoneNumber,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    PaymentStatus = order.PaymentStatus,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                };

                Log.Information("Order created successfully for user ID {UserId} with order ID {OrderId}.", order.UserId, order.Id);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, orderDtoResponse);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating order for user ID {UserId}.", orderDto.UserId);
                throw;
            }
        }


        // Update order status (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    Log.Warning("Order with ID {OrderId} not found for status update.", id);
                    return NotFound(new { message = "Order not found." });
                }

                order.Status = status;
                await _context.SaveChangesAsync();

                Log.Information("Order status updated successfully for order ID {OrderId}.", id);
                return Ok(new { message = "Order status updated successfully." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating status for order ID {OrderId}.", id);
                throw;
            }
        }

        // Delete an order (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    Log.Warning("Order with ID {OrderId} not found for deletion.", id);
                    return NotFound(new { message = "Order not found." });
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                Log.Information("Order with ID {OrderId} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting order with ID {OrderId}.", id);
                throw;
            }
        }
    }
}
