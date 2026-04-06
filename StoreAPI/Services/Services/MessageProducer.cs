using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Services.Interfaces;

namespace Services.Rabbit;

public class MessageProducer : IMessageProducer, IDisposable
{
    private int maxRetries = 5;
    private int delayMilliseconds = 3000;
    
    private readonly ILogger<MessageProducer> _logger;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _exchangeName;

    public MessageProducer(ILogger<MessageProducer> logger, IConfiguration configuration)
    {
        _logger = logger;

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                _exchangeName = configuration["Rabbit:Exchange"];
                
                var queueName = configuration["Rabbit:Queue"];
                var host =  configuration["Rabbit:Host"];
                var port = int.Parse(configuration["Rabbit:Port"] ?? "5672");
                var userName = configuration["Rabbit:UserName"];
                var password = configuration["Rabbit:Password"];
                ;
                _logger.LogInformation($"Connecting to RabbitMQ! Host: {host},  Port: {port}, UserName: {userName}");
                var factory = new ConnectionFactory
                {
                    HostName = configuration["Rabbit:Host"] ?? "localhost",
                    Port =  int.Parse(configuration["Rabbit:Port"] ?? "5672"),
                    UserName = configuration["Rabbit:UserName"] ?? "guest",
                    Password = configuration["Rabbit:Password"] ?? "guest",
                };

                _logger.LogInformation("Creating RabbitMQ Client");
                _connection = factory.CreateConnectionAsync().Result;
                _channel = _connection.CreateChannelAsync().Result;

                _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Direct);

                _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBindAsync(queue: queueName, exchange: _exchangeName, routingKey: "order.created");
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
            {
                _logger.LogInformation($"RabbitMQ not ready. Retrying in {delayMilliseconds / 1000} seconds... ({i +1})");

                if (i == maxRetries - 1)
                    throw;

                Task.Delay(delayMilliseconds).Wait();
            }
        }
    }

    public void PublishMessage<T>(T message, string routingKey)
    {
        if (_channel is null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized");
        
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties()
        {
            Persistent = true
        };
        _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body); 
    }
    
    public void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
    }
}