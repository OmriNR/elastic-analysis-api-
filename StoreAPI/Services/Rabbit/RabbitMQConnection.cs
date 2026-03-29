using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Services.Rabbit;

public class RabbitMQConnection : IRabbitMQConnectionManager, IDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMQConnection(IConfiguration configuration)
    {
        _factory = new ConnectionFactory()
        {
            HostName = configuration["Rabbit:Host"],
            Port = int.Parse(configuration["Rabbit:Port"]),
            UserName = configuration["Rabbit:User"],
            Password = configuration["Rabbit:Password"],
        };
    }
    
    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            _connection = await _factory.CreateConnectionAsync();
        }
        return _connection;
    }
    
    public void Dispose()
    {
        _connection?.Dispose();
    }
}