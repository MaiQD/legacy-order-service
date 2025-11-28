using Microsoft.Data.Sqlite;
using LegacyOrderService.Models;

namespace LegacyOrderService.Data
{
    public class OrderRepository : IOrderRepository
    {
        private string _connectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, "orders.db")}";


        public async Task SaveAsync(Order order)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Orders (CustomerName, ProductName, Quantity, Price)
                VALUES (@CustomerName, @ProductName, @Quantity, @Price)";
            
            command.Parameters.AddWithValue("@CustomerName", order.CustomerName);
            command.Parameters.AddWithValue("@ProductName", order.ProductName);
            command.Parameters.AddWithValue("@Quantity", order.Quantity);
            command.Parameters.AddWithValue("@Price", order.Price);

            await command.ExecuteNonQueryAsync();
        }
    }
}
