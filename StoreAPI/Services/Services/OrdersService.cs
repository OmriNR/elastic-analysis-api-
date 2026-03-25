using Domain;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class OrdersService : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductsRepository _productsRepository;
    private readonly IDiscountsRepository _discountsRepository;

    public OrdersService(IOrdersRepository ordersRepository, IUserRepository userRepository, IProductsRepository productsRepository, IDiscountsRepository discountsRepository)
    {
        _ordersRepository = ordersRepository;
        _userRepository = userRepository;
        _productsRepository = productsRepository;
        _discountsRepository = discountsRepository;
    }

    public Order GetOrder(string id, out Statuses status, out string error)
    {
        status = Statuses.OK;
        error = string.Empty;
        
        var order = _ordersRepository.GetOrder(id);

        if (order == null)
        {
            status = Statuses.NOT_FOUND;
            error = $"Order {id} not found";
        }
        
        return order;
    }

    public List<Order> GetOrdersByCustomer(string customerId, out Statuses status, out string error)
    {
        status = Statuses.OK;
        error = string.Empty;

        var user = _userRepository.GetUser(customerId);

        if (user == null)
        {
            status = Statuses.NOT_FOUND;
            error = $"User {customerId} not found";
            return null;
        }
        
        var orders = _ordersRepository.GetOrdersByCustomer(customerId);

        if (orders.Count == 0)
        {
            status = Statuses.NOT_FOUND;
            error = $"{customerId} does not have any orders";
            return null;
        }

        return orders;
    }

    public List<Order> GetOrdersByProduct(string productId, out Statuses status, out string error)
    {
        status = Statuses.OK;
        error = string.Empty;
        
        var product = _productsRepository.GetProduct(productId);
        if (product == null)
        {
            status = Statuses.NOT_FOUND;
            error = $"Product {productId} not found";
            return null;
        }
        
        var orders = _ordersRepository.GetOrdersByProduct(productId);

        if (orders.Count == 0)
        {
            status = Statuses.NOT_FOUND;
            error = $"Product {productId} does not have any orders";
            return null;
        }

        return orders;
    }

    public Order CreateOrder(Order order, out Statuses status, out string error)
    {
        status = Statuses.OK;
        error = string.Empty;

        if (!IsOrderValid(order, out error))
        {
            status = Statuses.INVALID;
            return null;
        }

        var customer = _userRepository.GetUser(order.CustomerID);

        if (customer == null)
        {
            status = Statuses.NOT_FOUND;
            error = $"User {order.CustomerID} not found";
        }

        order.TotalAmount = 0;
        order.DiscountApplied = false;
        
        foreach (var productId in order.Items)
        {
            var product = _productsRepository.GetProduct(productId);
            if (product == null)
            {
                status = Statuses.NOT_FOUND;
                error = $"Product {productId} not found";
                return null;
            }

            if (product.OwnerId == order.CustomerID)
            {
                status = Statuses.INVALID;
                error = $"User {product.OwnerId} can't order his own product";
                return null;
            }

            if (product.Quantity == 0)
            {
                status = Statuses.INVALID;
                error = $"Product {productId} is out of stock";
                return null;
            }
            
            var discount =  _discountsRepository.GetDiscountByProduct(productId);

            if (discount != null)
            {
                order.DiscountApplied = true;
                order.TotalAmount += product.Price * (1 - discount.Percentage / 100);
            }
            else
                order.TotalAmount += product.Price;
        }
        
        var orderId = Guid.NewGuid().ToString();
        order.OrderId = orderId;
        
        _ordersRepository.CreateOrder(order);
        
        var newOrder = _ordersRepository.GetOrder(order.OrderId);
        
        newOrder!.Items.ForEach(productId =>
        {
            var product = _productsRepository.GetProduct(productId);
            product!.Quantity--;
            _productsRepository.UpdateProduct(product);
        });
        
        return newOrder!;
    }

    private bool IsOrderValid(Order order, out string error)
    {
        error = string.Empty;
        
        if (order.CustomerID == null || order.CustomerID.Length == 0)
        {
            error = "CustomerId is Required";
            return false;
        }

        if (order.PaymentMethod == null || order.PaymentMethod.Length == 0)
        {
            error = "PaymentMethod is Required";
            return false;
        }

        if (order.Items.Count == 0)
        {
            error = "Items are Required and can't be empty";
            return false;
        }
        
        return true;
    }
}