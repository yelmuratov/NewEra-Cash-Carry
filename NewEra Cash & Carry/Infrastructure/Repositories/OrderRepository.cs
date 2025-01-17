using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Application.Interfaces.OrderInterfaces;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly RetailOrderingSystemDbContext _context;

        public OrderRepository(RetailOrderingSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order> GetOrderWithDetailsByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
