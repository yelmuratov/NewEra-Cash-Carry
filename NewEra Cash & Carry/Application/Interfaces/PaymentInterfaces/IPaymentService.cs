using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<(string PaymentIntentId, string Message)> ProcessPaymentAsync(int orderId);
        Task<(string RefundId, string Message)> RefundPaymentAsync(int orderId);
    }
}
