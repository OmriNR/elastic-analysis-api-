using Shared.Models;

namespace Services.Interfaces;

public interface IOrdersService
{
    Order GetOrder(string id, out Statuses status, out string error);
    List<Order> GetOrdersByCustomer(string customerId, out Statuses status, out string error);
    List<Order> GetOrdersByProduct(string productId, out Statuses status, out string error);
    Order CreateOrder(Order order, out Statuses status, out string error);
}