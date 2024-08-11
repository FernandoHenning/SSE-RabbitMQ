namespace SSE.Orders.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public string ProductDescription { get; set; }
    public string ShippingAddress { get; set; }
}