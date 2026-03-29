using RabbitMQ.Client;
using Services.Interfaces;

namespace Services.Rabbit;

public class MessageProducer : IMessageProducer
{
    private readonly IRabbitMQConnectionManager _connectionManager;

    public MessageProducer(IRabbitMQConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        var connection = await _connectionManager.GetConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        
        var json = System.Text.Json.JsonSerializer.Serialize(message);
        var body = System.Text.Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
    }
}