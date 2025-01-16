using NewEra_Cash___Carry.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Application.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
        Task<Order> GetOrderWithDetailsByIdAsync(int id);
    }
}
