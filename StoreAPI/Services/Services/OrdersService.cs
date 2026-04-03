using Domain;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class OrdersService : IOrdersService
{
    private const string queue = "orders_queue";
    private readonly ILogger<IOrdersService> _logger;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductsRepository _productsRepository;
    private readonly IDiscountsRepository _discountsRepository;
    private readonly IMessageProducer _messageProducer;

    public OrdersService(ILogger<IOrdersService> logger, IOrdersRepository ordersRepository, IUserRepository userRepository, IProductsRepository productsRepository, IDiscountsRepository discountsRepository, IMessageProducer messageProducer)
    {
        _logger = logger;
        _ordersRepository = ordersRepository;
        _userRepository = userRepository;
        _productsRepository = productsRepository;
        _discountsRepository = discountsRepository;
        _messageProducer = messageProducer;
    }

    public Order GetOrder(string id, out Statuses status, out string error)
    {
        _logger.LogInformation("Get order {id}", id);
        status = Statuses.OK;
        error = string.Empty;
        
        var order = _ordersRepository.GetOrder(id);

        if (order == null)
        {
            _logger.LogError($"Order {id} not found");
            status = Statuses.NOT_FOUND;
            error = $"Order {id} not found";
        }
        
        return order;
    }

    public List<Order> GetOrdersByCustomer(string customerId, out Statuses status, out string error)
    {
        _logger.LogInformation("Get orders by customer {customerId}", customerId);
        status = Statuses.OK;
        error = string.Empty;

        var user = _userRepository.GetUser(customerId);

        if (user == null)
        {
            _logger.LogError($"User {customerId} not found");
            status = Statuses.NOT_FOUND;
            error = $"User {customerId} not found";
            return null;
        }
        
        var orders = _ordersRepository.GetOrdersByCustomer(customerId);

        if (orders.Count == 0)
        {
            _logger.LogError($"User {customerId} doesn't have any orders");
            status = Statuses.NOT_FOUND;
            error = $"{customerId} does not have any orders";
            return null;
        }

        return orders;
    }

    public List<Order> GetOrdersByProduct(string productId, out Statuses status, out string error)
    {
        _logger.LogInformation("Get orders by product {productId}", productId);
        status = Statuses.OK;
        error = string.Empty;
        
        var product = _productsRepository.GetProduct(productId);
        if (product == null)
        {
            _logger.LogError($"Product {productId} not found");
            status = Statuses.NOT_FOUND;
            error = $"Product {productId} not found";
            return null;
        }
        
        var orders = _ordersRepository.GetOrdersByProduct(productId);

        if (orders.Count == 0)
        {
            _logger.LogError($"Product {productId} doesn't have any orders");
            status = Statuses.NOT_FOUND;
            error = $"Product {productId} does not have any orders";
            return null;
        }

        return orders;
    }

    public Order CreateOrder(Order order, out Statuses status, out string error)
    {
        _logger.LogInformation("Create order {order}", order);
        status = Statuses.OK;
        error = string.Empty;

        if (!IsOrderValid(order, out error))
        {
            _logger.LogError($"Order {order} is not valid", error);
            status = Statuses.INVALID;
            return null;
        }
        
        _logger.LogInformation("Order passed validation");
        var customer = _userRepository.GetUser(order.Customer.UserId);

        if (customer == null)
        {
            _logger.LogError($"User {order.Customer.UserId} doesn't exist");
            status = Statuses.NOT_FOUND;
            error = $"User {order.Customer.UserId} not found";
            return null;
        }

        _logger.LogInformation("Order has valid customer");
        order.TotalAmount = 0;
        order.DiscountApplied = false;
        
        foreach (var product in order.Items)
        {
            var check = _productsRepository.GetProduct(product.ProductId);
            if (check == null)
            {
                _logger.LogError($"Product {product.ProductId} not found");
                status = Statuses.NOT_FOUND;
                error = $"Product {product.ProductId} not found";
                return null;
            }
            
            if (product.OwnerId == order.Customer.UserId)
            {
                _logger.LogError($"User {order.Customer.UserId} can't buy his own products");
                status = Statuses.INVALID;
                error = $"User {product.OwnerId} can't order his own product";
                return null;
            }

            if (check.Quantity == 0)
            {
                _logger.LogError($"Product {product.ProductId} is out of stock");
                status = Statuses.INVALID;
                error = $"Product {product.ProductId} is out of stock";
                return null;
            }
            
            _logger.LogInformation($"{product.ProductId} passed and can be ordered");
        }
        
        var orderId = Guid.NewGuid().ToString();
        order.OrderId = orderId;
        
        _ordersRepository.CreateOrder(order);
        
        _logger.LogInformation($"Order {orderId} created, publishing it.");
        PublishOrder(order).Wait();
        
        _logger.LogInformation("Updating stock of products");
        order!.Items.ForEach(product =>
        {
            product!.Quantity--;
            _productsRepository.UpdateProduct(product);
        });
        
        var newOrder = _ordersRepository.GetOrder(order.OrderId);
        
        _logger.LogInformation($"Order created successfully. OrderId: {newOrder.OrderId}");
        return newOrder!;
    }

    private bool IsOrderValid(Order order, out string error)
    {
        error = string.Empty;
        
        if (order.Customer == null)
        {
            error = "Customer is Required";
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

    private async Task PublishOrder(Order order)
    {
        _logger.LogInformation("Publish order {order}", order.OrderId);

        await _messageProducer.SendMessageAsync(order, queue);
    }
}