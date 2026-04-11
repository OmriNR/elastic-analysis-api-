using Domain;

namespace Repositories.Interfaces;

public interface IOrdersRepository
{
    void CreateOrder(Order order);
    Order? GetOrder(string id);
    List<Order> GetOrdersByCustomer(string customerId);
    List<Order> GetOrdersByProduct(string productId);
}