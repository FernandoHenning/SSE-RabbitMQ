using Microsoft.Data.Sqlite;
using SSE.Orders.Models;

namespace SSE.Orders.Repository;

public class OrderRepository(IConfiguration configuration, ILogger<OrderRepository> logger)
    : IOrderRepository
{
    
    public void CreateOrder(Order order)
    {
        const string query = @"insert into Orders(OrderNumber, OrderDate, ProductDescription, ShippingAddress) values ($OrderNumber, $OrderDate, $ProductDescription, $ShippingAddress)";
        try
        {
            using var connection = new SqliteConnection(configuration.GetConnectionString("default"));
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("$OrderNumber", order.OrderNumber);
            command.Parameters.AddWithValue("$OrderDate", order.OrderDate);
            command.Parameters.AddWithValue("$ProductDescription", order.ProductDescription);
            command.Parameters.AddWithValue("$ShippingAddress", order.ShippingAddress);
            command.ExecuteNonQuery();
        }
        catch (SqliteException e)
        {
            logger.LogError("${Message}", e.Message);
            throw;
        }
    }

    public IEnumerable<Order> GetOrders()
    {
        const string query = @"select * from Orders";
        var orders = new List<Order>();
        try
        {
            using var connection = new SqliteConnection(configuration.GetConnectionString("default"));
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var number = reader.GetString(1);
                var date = reader.GetDateTime(2);
                var productDescription = reader.GetString(3);
                var shippingAddress = reader.GetString(4);
                
                orders.Add( new Order
                {
                    Id = id,
                    OrderNumber = number,
                    OrderDate = date,
                    ProductDescription = productDescription,
                    ShippingAddress = shippingAddress
                });
            }
        }
        catch (SqliteException e)
        {
            logger.LogError("${Message}", e.Message);
            throw;
        }

        return orders;
    }
}