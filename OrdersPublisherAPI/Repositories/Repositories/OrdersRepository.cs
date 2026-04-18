using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repositories;

public class OrdersRepository
{
    private readonly AppDBContext _context;

    public OrdersRepository(AppDBContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetRecentOrders()
    {
        var cutoff = DateTimeOffset.UtcNow.AddHours(-24);

        var recentOrders = await _context.Orders.Where(o => o.Timestamp >= cutoff).ToListAsync();
        
        return  recentOrders;
    }

    public async Task<int> GetRecentOrdersCount()
    {
        var recentOrders = await GetRecentOrders();

        return recentOrders.Count;
    }
}