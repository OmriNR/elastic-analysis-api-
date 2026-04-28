using System.Text;
using System.Text.Json.Nodes;
using Elastic.Clients.Elasticsearch;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Listener;

public class Worker(ILogger<Worker> logger, IConfiguration configuration, ElasticsearchClient elastic) : BackgroundService
{
    private static readonly Dictionary<string, string> QueueIndexMap = new()
    {
        ["orders_queue"]    = "orders",
        ["products_queue"]  = "products",
        ["users_queue"]     = "users",
        ["discounts_queue"] = "discounts"
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["Rabbit:Host"] ?? "localhost",
            Port = int.Parse(configuration["Rabbit:Port"] ?? "5672"),
            UserName = configuration["Rabbit:UserName"] ?? "guest",
            Password = configuration["Rabbit:Password"] ?? "guest"
        };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        foreach (var (queue, index) in QueueIndexMap)
        {
            await channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            var capturedQueue = queue;
            var capturedIndex = index;

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                logger.LogInformation("Received from '{Queue}': {Message}", capturedQueue, message);

                var doc = JsonNode.Parse(message);
                var response = await elastic.IndexAsync(doc, r => r.Index(capturedIndex), stoppingToken);

                if (response.IsSuccess())
                    logger.LogInformation("Indexed document to '{Index}' with id '{Id}'", capturedIndex, response.Id);
                else
                    logger.LogError("Failed to index to '{Index}': {Error}", capturedIndex, response.DebugInformation);

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            };

            await channel.BasicConsumeAsync(queue: queue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
            logger.LogInformation("Listening on queue '{Queue}' → index '{Index}'", queue, index);
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
