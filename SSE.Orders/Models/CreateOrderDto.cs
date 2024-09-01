using System.ComponentModel.DataAnnotations;

namespace SSE.Orders.Models;

public record CreateOrderDto(
    [Required]
    string OrderNumber,
    DateTime OrderDate,
    string ProductDescription,
    string ShippingAddress);