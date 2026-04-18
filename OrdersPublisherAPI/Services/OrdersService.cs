using Domain;
using Microsoft.Extensions.Logging;
using Repositories.Repositories;

namespace Services;

public class OrdersService
{
    private readonly OrdersRepository _ordersRepository;
    private readonly ILogger<OrdersService> _logger;

    public OrdersService(OrdersRepository ordersRepository, ILogger<OrdersService> logger)
    {
        _ordersRepository = ordersRepository;
        _logger = logger;
    }

    public async Task<int> GetRecentOrdersCount()
    {
        _logger.LogInformation("Getting Pending orders count");

        return await _ordersRepository.GetRecentOrdersCount();
    }

    public async Task<List<Order>> GetRecentOrders()
    {
        _logger.LogInformation("Getting pending orders");
        return await _ordersRepository.GetRecentOrders();
    }
}