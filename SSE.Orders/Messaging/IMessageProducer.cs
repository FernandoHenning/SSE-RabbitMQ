namespace SSE.Orders.Messaging;

public interface IMessageProducer
{
    Task SendNewOrderMessageAsync(string message);
}