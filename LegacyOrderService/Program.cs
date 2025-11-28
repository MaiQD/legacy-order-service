using LegacyOrderService.Data;
using LegacyOrderService.Services;

namespace LegacyOrderService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to Order Processor!");
            Console.WriteLine("Enter customer name:");
            string name = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter product name:");
            string product = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter quantity:");
            int qty = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Processing order...");

            var orderRepository = new OrderRepository();
            var productRepository = new ProductRepository();
            var orderService = new OrderService(orderRepository, productRepository);

            var order = await orderService.ProcessOrderAsync(name, product, qty);

            Console.WriteLine("Order complete!");
            Console.WriteLine("Customer: " + order.CustomerName);
            Console.WriteLine("Product: " + order.ProductName);
            Console.WriteLine("Quantity: " + order.Quantity);
            Console.WriteLine("Total: $" + order.Total);
            Console.WriteLine("Done.");
        }
    }
}
