using FluentValidation;
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
            string? quantityInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(quantityInput) || !int.TryParse(quantityInput, out int qty))
            {
                Console.WriteLine("Error: Quantity must be a valid integer.");
                return;
            }

            Console.WriteLine("Processing order...");

            var orderService = serviceProvider.GetRequiredService<OrderService>();

            try
            {
                var order = await orderService.ProcessOrderAsync(name, product, qty);

                Console.WriteLine("Order complete!");
                Console.WriteLine("Customer: " + order.CustomerName);
                Console.WriteLine("Product: " + order.ProductName);
                Console.WriteLine("Quantity: " + order.Quantity);
                Console.WriteLine("Total: $" + order.Total);
                Console.WriteLine("Done.");
            }
            catch (ValidationException ex)
            {
                Console.WriteLine("Validation errors:");
                foreach (var error in ex.Errors)
                {
                    Console.WriteLine($"  - {error.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing order: {ex.Message}");
            }
        }
    }
}
