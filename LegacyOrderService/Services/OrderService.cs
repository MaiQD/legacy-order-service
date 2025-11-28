using LegacyOrderService.Data;
using LegacyOrderService.Models;

namespace LegacyOrderService.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Order> ProcessOrderAsync(string customerName, string productName, int quantity)
        {
            var price = _productRepository.GetPrice(productName);

            var order = new Order
            {
                CustomerName = customerName,
                ProductName = productName,
                Quantity = quantity,
                Price = price
            };

            await _orderRepository.SaveAsync(order);

            return order;
        }
    }
}

