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
    private readonly IConnection _connection;
    private readonly IChannel _channel;

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

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public void PublishMessage<T>(T message, string queue)
    {
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
