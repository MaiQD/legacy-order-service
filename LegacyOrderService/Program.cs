using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LegacyOrderService.Data;
using LegacyOrderService.Services;
using LegacyOrderService.Configuration;

namespace LegacyOrderService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();

            services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<OrderService>();

            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("Welcome to Order Processor!");
            Console.WriteLine("Enter customer name:");
            string name = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter product name:");
            string product = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter quantity:");
            int qty = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Processing order...");

            var orderService = serviceProvider.GetRequiredService<OrderService>();

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
