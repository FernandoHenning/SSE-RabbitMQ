using System.Text;
using RabbitMQ.Client;

namespace SSE.Orders.Messaging;

public class MessageProducer :  IMessageProducer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string? _queueName;
    private readonly ILogger<MessageProducer> _logger;

    public MessageProducer(IConfiguration configuration, ILogger<MessageProducer> logger)
    {
        _logger = logger;

        var rabbitMqSection = configuration.GetSection("RabbitMq");
        var factory = new ConnectionFactory()
        {
            HostName = rabbitMqSection["HostName"],
            UserName = rabbitMqSection["UserName"],
            Password = rabbitMqSection["Password"],
            VirtualHost = rabbitMqSection["VHost"]
        };
        _queueName = rabbitMqSection["QueueName"];

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public Task SendNewOrderMessageAsync(string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));

        var body = Encoding.UTF8.GetBytes(message);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: "",
            routingKey: _queueName,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("[x] New orders message queued: {Message}", message);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}