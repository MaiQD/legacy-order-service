using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using LegacyOrderService.Models;
using LegacyOrderService.Configuration;

namespace LegacyOrderService.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IOptions<DatabaseOptions> options)
        {
            var connectionString = options.Value.DefaultConnection;
            var builder = new SqliteConnectionStringBuilder(connectionString);
            
            if (!string.IsNullOrEmpty(builder.DataSource) && !Path.IsPathRooted(builder.DataSource))
            {
                builder.DataSource = Path.Combine(AppContext.BaseDirectory, builder.DataSource);
            }
            
            _connectionString = builder.ConnectionString;
        }

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
