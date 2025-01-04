using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.Helpers;
using Stripe;

namespace NewEra_Cash___Carry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;
        private readonly PaymentSettings _paymentSettings;

        public PaymentsController(RetailOrderingSystemDbContext context, IOptions<PaymentSettings> paymentSettings)
        {
            _context = context;
            _paymentSettings = paymentSettings.Value;
            StripeConfiguration.ApiKey = _paymentSettings.SecretKey;
        }

        // POST: api/Payments/charge
        [HttpPost("charge")]
        public async Task<IActionResult> ProcessPayment(int orderId)
        {
            // Fetch the order from the database
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            // Check if the order is already paid
            if (order.PaymentStatus == "Paid")
            {
                return BadRequest(new { message = "Order is already paid." });
            }

            try
            {
                // Create a payment intent
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
                {
                    Amount = (long)(order.TotalAmount * 100), // Convert to cents
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                });

                // Update the payment status and save the PaymentIntentId in the database
                order.PaymentStatus = "Paid";
                order.PaymentIntentId = paymentIntent.Id; // Save the payment intent ID
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Payment processed successfully.",
                    PaymentIntentId = paymentIntent.Id
                });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // POST: api/Payments/refund
        [HttpPost("refund")]
        public async Task<IActionResult> RefundPayment(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            // Check if the order has been paid
            if (order.PaymentStatus != "Paid")
            {
                return BadRequest(new { message = "Order has not been paid." });
            }

            try
            {
                // Create a refund
                var refundService = new RefundService();
                var refund = refundService.Create(new RefundCreateOptions
                {
                    PaymentIntent = order.PaymentIntentId // Use the saved PaymentIntentId
                });

                // Update the payment status
                order.PaymentStatus = "Refunded";
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Payment refunded successfully.",
                    RefundId = refund.Id
                });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
