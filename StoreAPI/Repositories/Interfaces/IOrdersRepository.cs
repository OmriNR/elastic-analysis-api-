using Domain;

namespace Repositories.Interfaces;

public interface IOrdersRepository
{
    void CreateOrder(Order order);
    void UpdateOrder(Order order);
    void DeleteOrder(Order order);
    Order GetOrder(int id);
    List<Order> GetOrders(List<string> ids);
    List<Order> GetOrdersByCustomer(string customerId);
    List<Order> GetOrdersByProduct(string productId);
}