using AutoMapper;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.DTOs.order;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.DTOs.order.NewEra_Cash___Carry.DTOs.order;

namespace NewEra_Cash___Carry.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IProductRepository productRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderWithDetailsByIdAsync(id);
            if (order == null) throw new KeyNotFoundException("Order not found.");

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(OrderCreateDto orderDto)
        {
            var user = await _userRepository.GetByIdAsync(orderDto.UserId);
            if (user == null) throw new KeyNotFoundException("User not found.");

            var order = new Order
            {
                UserId = orderDto.UserId,
                Status = "Pending",
                PaymentStatus = "Pending",
                OrderItems = new List<OrderItem>()
            };

            foreach (var itemDto in orderDto.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null) throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} not found.");
                if (product.Stock < itemDto.Quantity) throw new Exception($"Not enough stock for product {product.Name}.");

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

            if (order.TotalAmount <= 0) throw new Exception("Order total amount must be greater than 0.");

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) throw new KeyNotFoundException("Order not found.");

            order.Status = status;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) throw new KeyNotFoundException("Order not found.");

            _orderRepository.Delete(order);
            await _orderRepository.SaveChangesAsync();
        }
    }
}
