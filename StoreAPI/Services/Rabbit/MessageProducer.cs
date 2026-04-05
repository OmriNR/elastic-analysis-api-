using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Services.Interfaces;

namespace Services.Rabbit;

public class MessageProducer : IMessageProducer
{
    private readonly IConnectionFactory _factory;
    private readonly ILogger<MessageProducer> _logger;

    public MessageProducer(IConfiguration config, ILogger<MessageProducer> logger)
    {
        _logger = logger;
        _factory = new ConnectionFactory()
        {
            HostName = config["Rabbit:HostName"],
            Port = int.Parse(config["Rabbit:Port"]),
            UserName = config["Rabbit:UserName"],
            Password = config["Rabbit:Password"]
        };
    }

    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        try
        {
            _logger.LogInformation("Publishing order {order}", queueName);
        
            var connection = await _factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                durable: false, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null);
        
            var json = System.Text.Json.JsonSerializer.Serialize(message);
            var body = System.Text.Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
            
            _logger.LogInformation("Published order for {queue}", queueName, queueName);
            _logger.LogInformation("Message body: " + json);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Failed publish to queue: " + ex.Message);
        }
    }
}