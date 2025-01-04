using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.Helpers;
using Stripe;

namespace NewEra_Cash___Carry.Controllers
{
    /// <summary>
    /// Controller for handling payment processing and refunds.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;
        private readonly PaymentSettings _paymentSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentsController"/> class.
        /// </summary>
        /// <param name="context">Database context for interacting with orders.</param>
        /// <param name="paymentSettings">Payment settings configuration.</param>
        public PaymentsController(RetailOrderingSystemDbContext context, IOptions<PaymentSettings> paymentSettings)
        {
            _context = context;
            _paymentSettings = paymentSettings.Value;
            StripeConfiguration.ApiKey = _paymentSettings.SecretKey;
        }

        /// <summary>
        /// Processes a payment for a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order to process payment for.</param>
        /// <returns>Confirmation of the payment process.</returns>
        /// <remarks>
        /// - This endpoint will create a Stripe payment intent.
        /// - Payment status is updated to "Paid" if successful.
        /// </remarks>
        /// <response code="200">Returns a success message and payment intent ID.</response>
        /// <response code="404">If the order is not found.</response>
        /// <response code="400">If the order is already paid or a Stripe exception occurs.</response>
        [HttpPost("charge")]
        public async Task<IActionResult> ProcessPayment(int orderId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            if (order.PaymentStatus == "Paid")
            {
                return BadRequest(new { message = "Order is already paid." });
            }

            try
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
                {
                    Amount = (long)(order.TotalAmount * 100), // Convert to cents
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                });

                order.PaymentStatus = "Paid";
                order.PaymentIntentId = paymentIntent.Id;
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

        /// <summary>
        /// Refunds a payment for a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order to refund payment for.</param>
        /// <returns>Confirmation of the refund process.</returns>
        /// <remarks>
        /// - This endpoint will create a Stripe refund for a paid order.
        /// - Payment status is updated to "Refunded" if successful.
        /// </remarks>
        /// <response code="200">Returns a success message and refund ID.</response>
        /// <response code="404">If the order is not found.</response>
        /// <response code="400">If the order is not paid or a Stripe exception occurs.</response>
        [HttpPost("refund")]
        public async Task<IActionResult> RefundPayment(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            if (order.PaymentStatus != "Paid")
            {
                return BadRequest(new { message = "Order has not been paid." });
            }

            try
            {
                var refundService = new RefundService();
                var refund = refundService.Create(new RefundCreateOptions
                {
                    PaymentIntent = order.PaymentIntentId
                });

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
