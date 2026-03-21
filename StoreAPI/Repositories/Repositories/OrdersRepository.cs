using Domain;
using Repositories.Interfaces;

namespace Repositories.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly AppDBContext _context;

    public OrdersRepository(AppDBContext context)
    {
        _context = context;
    }

    public void CreateOrder(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    public Order? GetOrder(string id)
    {
        var order = _context.Orders.Find(id);
        return order;
    }

    public List<Order> GetOrdersByCustomer(string customerId)
    {
        var orders = _context.Orders.Where(o => o.CustomerID.Equals(customerId)).ToList();
        return orders;
    }

    public List<Order> GetOrdersByProduct(string productId)
    {
        var orders = _context.Orders.Where(o => o.Items.Contains(productId)).ToList();
        return orders;
    }
}