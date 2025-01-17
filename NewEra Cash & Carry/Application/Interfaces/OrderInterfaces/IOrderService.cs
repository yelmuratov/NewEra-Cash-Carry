using NewEra_Cash___Carry.Core.DTOs.order;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.DTOs.order.NewEra_Cash___Carry.DTOs.order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Application.Interfaces.OrderInterfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(OrderCreateDto orderDto);
        Task UpdateOrderStatusAsync(int id, string status);
        Task DeleteOrderAsync(int id);
    }
}
