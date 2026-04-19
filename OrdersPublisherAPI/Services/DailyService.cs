using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repositories.Repositories;

namespace Services;

public class DailyService : BackgroundService
{
    private readonly ILogger<DailyService> _logger;
    private readonly WorkerControlService _control;
    private readonly OrdersRepository _repository;

    public DailyService(ILogger<DailyService> logger, WorkerControlService control, OrdersRepository repository)
    {
        _logger = logger;
        _control = control;
        _repository = repository;
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
        throw new NotImplementedException();
    }
}