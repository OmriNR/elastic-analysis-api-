using System.ComponentModel;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repositories.Repositories;

namespace Services;

public class DailyService : BackgroundService
{
    private readonly ILogger<DailyService> _logger;
    private readonly WorkerControlService _control;
    private readonly OrdersRepository _repository;
    private readonly ElasticsearchClient _elasticsearch;
    
    public DailyService(ILogger<DailyService> logger, WorkerControlService control, OrdersRepository repository, IConfiguration config)
    {
        _logger = logger;
        _control = control;
        _repository = repository;
        
        string elasticUrl = config.GetSection("Elasticsearch:url").Value!;
        _elasticsearch = new ElasticsearchClient(new Uri(elasticUrl));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_control.IsEnabled)
            {
                _logger.LogInformation("Service is currently disabled");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            
            DateTime now =  DateTime.Now;
            DateTime nextRun = now.Date.Add(_control.ExecutionTimeout);

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            TimeSpan delay = nextRun - now;
            
            _logger.LogInformation("Waiting until {targetTime} (Delay: {delay})", nextRun, delay);

            try
            {
                await Task.Delay(delay, stoppingToken);

                if (_control.IsEnabled)
                {
                    await SendOrders();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed sending orders: " + e.Message);
                break;
            }
        }
    }

    private async Task SendOrders()
    {
        var orders = await _repository.GetRecentOrders();

        var bulkResponse = await _elasticsearch.BulkAsync(b => b
            .Index("my-orders-index").IndexMany(orders));

        if (!bulkResponse.IsValidResponse)
        {
            foreach (var itemWithError in bulkResponse.ItemsWithErrors)
            {
                _logger.LogError($"Failed to index document {itemWithError.Id}: {itemWithError.Error}");
            }
        }
    }
}