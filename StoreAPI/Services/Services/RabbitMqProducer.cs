using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Services.Interfaces;

namespace Services.Services;

public class RabbitMqProducer : IMessageProducer, IDisposable
{
    private readonly ILogger<RabbitMqProducer> _logger;
    private readonly IConnection? _connection;
    private readonly IChannel? _channel;

    public RabbitMqProducer(ILogger<RabbitMqProducer> logger, IConfiguration configuration)
    {
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = configuration["Rabbit:Host"] ?? "localhost",
            Port = int.Parse(configuration["Rabbit:Port"] ?? "5672"),
            UserName = configuration["Rabbit:UserName"] ?? "guest",
            Password = configuration["Rabbit:Password"] ?? "guest"
        };

        // IMessageProducer is a constructor dependency of most services (Users, Products, Orders,
        // Discounts), so a throwing connect here would take down virtually the entire API whenever
        // RabbitMQ isn't deployed. Degrade to a disabled producer instead — PublishMessage becomes
        // a logged no-op — so the rest of the app keeps working without a broker configured.
        try
        {
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Could not connect to RabbitMQ at {Host}:{Port}. Message publishing is disabled.",
                factory.HostName, factory.Port);
        }
    }

    public void PublishMessage<T>(T message, string queue)
    {
        if (_channel == null)
        {
            _logger.LogWarning("Skipped publishing to queue '{Queue}': RabbitMQ is not connected.", queue);
            return;
        }

        _channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null).GetAwaiter().GetResult();

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var props = new BasicProperties { Persistent = true };

        _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queue,
            mandatory: false,
            basicProperties: props,
            body: body).GetAwaiter().GetResult();

        _logger.LogInformation("Published message to queue '{Queue}'", queue);
    }

    public void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
    }
}
