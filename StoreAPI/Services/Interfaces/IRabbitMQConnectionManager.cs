using RabbitMQ.Client;

namespace Services.Rabbit;

public interface IRabbitMQConnectionManager
{
    Task<IConnection> GetConnectionAsync();
}