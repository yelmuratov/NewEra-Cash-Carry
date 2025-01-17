using NewEra_Cash___Carry.Core.Entities;

namespace NewEra_Cash___Carry.Application.Interfaces.PaymentInterfaces
{
    public interface IPaymentRepository
    {
        public interface IPaymentRepository
        {
            Task<Order> GetOrderByIdAsync(int orderId);
            Task UpdateOrderAsync(Order order);
        }
    }
}
