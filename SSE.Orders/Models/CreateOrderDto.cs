namespace SSE.Orders.Models;

public record CreateOrderDto(
    string OrderNumber,
    DateTime OrderDate,
    string ProductDescription,
    string ShippingAddress);