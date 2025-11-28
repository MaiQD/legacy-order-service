using FluentAssertions;
using LegacyOrderService.Configuration;
using LegacyOrderService.Data;
using LegacyOrderService.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace LegacyOrderServiceTests.Data;

public class OrderRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly OrderRepository _orderRepository;
    private readonly string _testDbPath;

    public OrderRepositoryTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_orders_{Guid.NewGuid()}.db");
        var connectionString = $"Data Source={_testDbPath}";
        
        InitializeDatabase(connectionString);

        var options = Options.Create(new DatabaseOptions
        {
            DefaultConnection = connectionString
        });

        _orderRepository = new OrderRepository(options);
        
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
    }

    private static void InitializeDatabase(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CustomerName TEXT NOT NULL,
                ProductName TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                Price REAL NOT NULL
            )";
        command.ExecuteNonQuery();
    }

    [Fact]
    public async Task SaveAsync_ValidOrder_SavesOrderToDatabase()
    {
        // Arrange
        var order = new Order
        {
            CustomerName = "Test Customer",
            ProductName = "Test Product",
            Quantity = 5,
            Price = 10.99
        };

        // Act
        await _orderRepository.SaveAsync(order);

        // Assert
        var verifyCommand = _connection.CreateCommand();
        verifyCommand.CommandText = @"
            SELECT CustomerName, ProductName, Quantity, Price 
            FROM Orders 
            WHERE CustomerName = @CustomerName";
        verifyCommand.Parameters.AddWithValue("@CustomerName", order.CustomerName);

        await using var reader = await verifyCommand.ExecuteReaderAsync();
        reader.Read().Should().BeTrue();
        reader.GetString(0).Should().Be(order.CustomerName);
        reader.GetString(1).Should().Be(order.ProductName);
        reader.GetInt32(2).Should().Be(order.Quantity);
        reader.GetDouble(3).Should().Be(order.Price);
    }

    [Fact]
    public async Task SaveAsync_MultipleOrders_SavesAllOrdersToDatabase()
    {
        // Arrange
        var order1 = new Order
        {
            CustomerName = "Customer 1",
            ProductName = "Product 1",
            Quantity = 2,
            Price = 5.50
        };

        var order2 = new Order
        {
            CustomerName = "Customer 2",
            ProductName = "Product 2",
            Quantity = 3,
            Price = 7.25
        };

        // Act
        await _orderRepository.SaveAsync(order1);
        await _orderRepository.SaveAsync(order2);

        // Assert
        var countCommand = _connection.CreateCommand();
        countCommand.CommandText = "SELECT COUNT(*) FROM Orders";
        var count = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
        count.Should().Be(2);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }
}

