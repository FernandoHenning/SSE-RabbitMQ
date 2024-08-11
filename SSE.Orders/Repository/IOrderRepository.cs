using SSE.Orders.Models;

namespace SSE.Orders.Repository;

public interface IOrderRepository
{
    void CreateOrder(Order order);
    IEnumerable<Order> GetOrders();
}