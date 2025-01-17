using Microsoft.Extensions.Options;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.Shared.Settings;
using Serilog;
using Stripe;

namespace NewEra_Cash___Carry.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly PaymentSettings _paymentSettings;

        public PaymentService(IRepository<Order> orderRepository, IOptions<PaymentSettings> paymentSettings)
        {
            _orderRepository = orderRepository;
            _paymentSettings = paymentSettings.Value;
            StripeConfiguration.ApiKey = _paymentSettings.SecretKey;
        }

        public async Task<(string PaymentIntentId, string Message)> ProcessPaymentAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new KeyNotFoundException("Order not found.");

            if (order.PaymentStatus == "Paid")
            {
                throw new InvalidOperationException("Order is already paid.");
            }

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = (long)(order.TotalAmount * 100), // Convert to cents
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
            });

            order.PaymentStatus = "Paid";
            order.PaymentIntentId = paymentIntent.Id;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            return (paymentIntent.Id, "Payment processed successfully.");
        }

        public async Task<(string RefundId, string Message)> RefundPaymentAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException("Order not found.");
                }

                if (order.PaymentStatus != "Paid")
                {
                    throw new InvalidOperationException("Order has not been paid.");
                }

                var refundService = new RefundService();
                var refund = refundService.Create(new RefundCreateOptions
                {
                    PaymentIntent = order.PaymentIntentId
                });

                order.PaymentStatus = "Refunded";
                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();

                return (refund.Id, "Payment refunded successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                // Log the specific key not found exception
                Log.Error(ex, "Refund failed: {Message}", ex.Message);
                return (null, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Log invalid operation errors (e.g., payment not paid)
                Log.Error(ex, "Refund failed: {Message}", ex.Message);
                return (null, ex.Message);
            }
            catch (StripeException ex)
            {
                // Log Stripe-specific errors
                Log.Error(ex, "Stripe refund failed: {Message}", ex.Message);
                return (null, $"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Catch any other unhandled exceptions
                Log.Error(ex, "An unexpected error occurred during refund: {Message}", ex.Message);
                return (null, "An unexpected error occurred while processing the refund.");
            }
        }

    }
}
