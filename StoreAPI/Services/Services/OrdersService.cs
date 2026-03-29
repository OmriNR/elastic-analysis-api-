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

        var customer = _userRepository.GetUser(order.CustomerID);

        if (customer == null)
        {
            _logger.LogError($"User {order.CustomerID} doesn't exist");
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
                _logger.LogError($"Product {productId} not found");
                status = Statuses.NOT_FOUND;
                error = $"Product {productId} not found";
                return null;
            }

            if (product.OwnerId == order.CustomerID)
            {
                _logger.LogError($"User {order.CustomerID} can't buy his own products");
                status = Statuses.INVALID;
                error = $"User {product.OwnerId} can't order his own product";
                return null;
            }

            if (product.Quantity == 0)
            {
                _logger.LogError($"Product {product.ProductId} is out of stock");
                status = Statuses.INVALID;
                error = $"Product {productId} is out of stock";
                return null;
            }
            
            var discount =  _discountsRepository.GetDiscountByProduct(productId);

            if (discount != null)
            {
                _logger.LogInformation($"Product {productId} is on  discount");
                order.DiscountApplied = true;
                order.TotalAmount += product.Price * (1 - discount.Percentage / 100);
            }
            else
                order.TotalAmount += product.Price;
        }
        
        var orderId = Guid.NewGuid().ToString();
        order.OrderId = orderId;
        
        _ordersRepository.CreateOrder(order);
        PublishOrder(order).Wait();
        
        var newOrder = _ordersRepository.GetOrder(order.OrderId);
        
        newOrder!.Items.ForEach(productId =>
        {
            var product = _productsRepository.GetProduct(productId);
            product!.Quantity--;
            _productsRepository.UpdateProduct(product);
        });
        
        _logger.LogInformation($"Order created successfully. OrderId: {newOrder.OrderId}");
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

    private async Task PublishOrder(Order order)
    {
        _logger.LogInformation("Publish order {order}", order.OrderId);
        
        OrderPublish publish = new OrderPublish();
        
        publish.OrderId = order.OrderId;
        publish.Timestamp = order.Timestamp;
        publish.TotalAmount = order.TotalAmount;
        publish.DiscountApplied = order.DiscountApplied;
        
        var customer = _userRepository.GetUser(order.CustomerID);
        publish.Customer = customer;

        publish.Items = new List<Product>();
        
        order.Items.ForEach(orderId =>
        {
            var product =  _productsRepository.GetProduct(orderId);
            if (product != null)
                publish.Items.Add(product);
        });

        await _messageProducer.SendMessageAsync(publish, queue);
    }
}