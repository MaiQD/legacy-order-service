using LegacyOrderService.Data;
using LegacyOrderService.Models;
using LegacyOrderService.Validators;
using FluentValidation;

namespace LegacyOrderService.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ProcessOrderRequestValidator _validator;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _validator = new ProcessOrderRequestValidator();
        }

        public async Task<Order> ProcessOrderAsync(string customerName, string productName, int quantity)
        {
            var request = new ProcessOrderRequest(customerName, productName, quantity);
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

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

