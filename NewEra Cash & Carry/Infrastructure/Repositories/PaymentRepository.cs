using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Application.Interfaces.PaymentInterfaces;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.Infrastructure.Data;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly RetailOrderingSystemDbContext _context;

        public PaymentRepository(RetailOrderingSystemDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
