using Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Services;

namespace TestProject1;

public class OrdersTests
{
    private const string ORDER_ID_NOT_FOUND = "OrderIDNotFound";
    private const string ORDER_ID_EXISTS = "OrderIdExists";

    private const string USER_ID_NOT_FOUND = "UserNotFound";
    private const string USER_ID_EXISTS = "UserExists";
    private const string USER_ID_NO_ORDERS = "UserNoOrdersExists";

    private const string PRODUCT_ID_NOT_FOUND = "ProductNotFound";
    private const string PRODUCT_ID_EXISTS = "ProductExists";
    private const string PRODUCT_ID_NO_ORDERS = "ProductNoOrdersExists";
    private const string PRODUCT_ID_NOT_ON_STOCK = "ProductNotOnStock";
    private const string PRODUCT_ID_OWNED_BY_CUSTOMER = "ProductOwnedByCustomer";

    private static Order validOrder = new Order()
    {
        OrderId = ORDER_ID_EXISTS,
        Customer = USER_ID_EXISTS,
        DiscountApplied = false,
        Items = new List<Product>() { new Product() { ProductId = PRODUCT_ID_EXISTS } },
        PaymentMethod = "CreditCard",
        Timestamp = DateTime.Now,
        TotalAmount = 100
    };

    private static Order invalid_order_no_customer = new Order()
    {
        OrderId = ORDER_ID_EXISTS,
        Customer = null,
        DiscountApplied = false,
        Items = new List<Product>() { new Product() { ProductId = PRODUCT_ID_EXISTS } },
        PaymentMethod = "CreditCard",
        Timestamp = DateTime.Now,
        TotalAmount = 100
    };

    private static Order invalid_order_no_payment = new Order()
    {
        OrderId = ORDER_ID_EXISTS,
        Customer = USER_ID_EXISTS,
        DiscountApplied = false,
        Items = new List<Product>() { new Product() { ProductId = PRODUCT_ID_EXISTS } },
        PaymentMethod = "",
        Timestamp = DateTime.Now,
        TotalAmount = 100
    };

    private static Order invalid_order_no_items = new Order()
    {
        OrderId = ORDER_ID_EXISTS,
        Customer = USER_ID_EXISTS,
        DiscountApplied = false,
        Items = new List<Product>(),
        PaymentMethod = "CreditCard",
        Timestamp = DateTime.Now,
        TotalAmount = 100
    };

    private static Order invalid_order_no_product_exist = new Order()
    {
        OrderId = ORDER_ID_EXISTS,
        Customer = USER_ID_EXISTS,
        DiscountApplied = false,
        Items = new List<Product>() { new Product() { ProductId = PRODUCT_ID_NOT_FOUND } },
        PaymentMethod = "CreditCard",
        Timestamp = DateTime.Now,
        TotalAmount = 100
    };

    private static Order invalid_order_not_in_stock = new Order()
    {
        OrderId = ORDER_ID_EXISTS,
        Customer = USER_ID_EXISTS,
        DiscountApplied = false,
        Items = new List<Product>() { new Product() { ProductId = PRODUCT_ID_NOT_ON_STOCK } },
        PaymentMethod = "CreditCard",
        Timestamp = DateTime.Now,
        TotalAmount = 100
    };

    // OwnerId must be set on the item because the service checks product.OwnerId from the order's Items list
    private static Order invalid_order_user_orders_own_product = new Order()
    {
        OrderId = ORDER_ID_EXISTS,
        Customer = USER_ID_EXISTS,
        DiscountApplied = false,
        Items = new List<Product>() { new Product() { ProductId = PRODUCT_ID_OWNED_BY_CUSTOMER, OwnerId = USER_ID_EXISTS } },
        PaymentMethod = "CreditCard",
        Timestamp = DateTime.Now,
        TotalAmount = 100
    };

    private static User validUser = new User() { UserId = USER_ID_EXISTS };

    private static Product validProduct = new Product()
    {
        ProductId = PRODUCT_ID_EXISTS,
        OwnerId = USER_ID_NO_ORDERS,
        Quantity = 10
    };

    private static Product validProductNotOnStock = new Product()
    {
        ProductId = PRODUCT_ID_NOT_ON_STOCK,
        OwnerId = USER_ID_NO_ORDERS,
        Quantity = 0
    };

    private static Product validProductByCustomer = new Product()
    {
        ProductId = PRODUCT_ID_OWNED_BY_CUSTOMER,
        OwnerId = USER_ID_EXISTS,
        Quantity = 10
    };

    private Mock<IProductsRepository> _productsRepositoryMock;
    private Mock<IUsersRepository> _userRepositoryMock;
    private Mock<IOrdersRepository> _ordersRepositoryMock;
    private Mock<IDiscountsRepository> _discountsRepositoryMock;
    private IOrdersService _service;

    [SetUp]
    public void Setup()
    {
        _productsRepositoryMock = new Mock<IProductsRepository>();
        _userRepositoryMock = new Mock<IUsersRepository>();
        _discountsRepositoryMock = new Mock<IDiscountsRepository>();
        _ordersRepositoryMock = new Mock<IOrdersRepository>();

        // Match any ID except ORDER_ID_NOT_FOUND so dynamically generated order IDs work in CreateOrder
        _ordersRepositoryMock.Setup(repo =>
            repo.GetOrder(It.Is<string>(id => id != ORDER_ID_NOT_FOUND))).Returns(validOrder);

        _ordersRepositoryMock.Setup(repo =>
            repo.GetOrder(ORDER_ID_NOT_FOUND)).Returns((Order)null);

        _ordersRepositoryMock.Setup(repo =>
            repo.GetOrdersByCustomer(USER_ID_NO_ORDERS)).Returns(new List<Order>());

        _ordersRepositoryMock.Setup(repo =>
            repo.GetOrdersByCustomer(USER_ID_EXISTS)).Returns(new List<Order>() { validOrder });

        _ordersRepositoryMock.Setup(repo =>
            repo.GetOrdersByProduct(PRODUCT_ID_NO_ORDERS)).Returns(new List<Order>());

        _ordersRepositoryMock.Setup(repo =>
            repo.GetOrdersByProduct(PRODUCT_ID_EXISTS)).Returns(new List<Order>() { validOrder });

        _userRepositoryMock.Setup(repo =>
            repo.GetUserById(USER_ID_EXISTS)).Returns(validUser);

        _userRepositoryMock.Setup(repo =>
            repo.GetUserById(USER_ID_NO_ORDERS)).Returns(validUser);

        _userRepositoryMock.Setup(repo =>
            repo.GetUserById(USER_ID_NOT_FOUND)).Returns((User)null);

        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(PRODUCT_ID_EXISTS)).Returns(validProduct);

        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(PRODUCT_ID_NO_ORDERS)).Returns(validProduct);

        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(PRODUCT_ID_NOT_ON_STOCK)).Returns(validProductNotOnStock);

        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(PRODUCT_ID_OWNED_BY_CUSTOMER)).Returns(validProductByCustomer);

        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(PRODUCT_ID_NOT_FOUND)).Returns((Product)null);

        ILogger<IOrdersService> logger = new NullLogger<IOrdersService>();
        _service = new OrdersService(logger, _ordersRepositoryMock.Object, _userRepositoryMock.Object, _productsRepositoryMock.Object, _discountsRepositoryMock.Object);
    }

    [Test, TestCaseSource(nameof(GetTestCases))]
    public void All_Gets_Tests(string id, string type, Statuses expectedStatus, string expectedError)
    {
        string error = string.Empty;
        Statuses status = Statuses.OK;

        switch (type)
        {
            case "ORDER":
                _service.GetOrder(id, out status, out error);
                break;
            case "USER":
                _service.GetOrdersByCustomer(id, out status, out error);
                break;
            case "PRODUCT":
                _service.GetOrdersByProduct(id, out status, out error);
                break;
        }

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [Test, TestCaseSource(nameof(CreateOrderTestCases))]
    public void Create_Order_Test(Order order, Statuses expectedStatus, string expectedError)
    {
        _service.CreateOrder(order, out Statuses status, out var error);

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    public static IEnumerable<TestCaseData> GetTestCases()
    {
        yield return new TestCaseData(ORDER_ID_EXISTS, "ORDER", Statuses.OK, string.Empty);
        yield return new TestCaseData(ORDER_ID_NOT_FOUND, "ORDER", Statuses.NOT_FOUND, $"Order {ORDER_ID_NOT_FOUND} not found");

        yield return new TestCaseData(PRODUCT_ID_EXISTS, "PRODUCT", Statuses.OK, string.Empty);
        yield return new TestCaseData(PRODUCT_ID_NOT_FOUND, "PRODUCT", Statuses.NOT_FOUND, $"Product {PRODUCT_ID_NOT_FOUND} not found");
        yield return new TestCaseData(PRODUCT_ID_NO_ORDERS, "PRODUCT", Statuses.NOT_FOUND, $"Product {PRODUCT_ID_NO_ORDERS} does not have any orders");

        yield return new TestCaseData(USER_ID_EXISTS, "USER", Statuses.OK, string.Empty);
        yield return new TestCaseData(USER_ID_NOT_FOUND, "USER", Statuses.NOT_FOUND, $"User {USER_ID_NOT_FOUND} not found");
        yield return new TestCaseData(USER_ID_NO_ORDERS, "USER", Statuses.NOT_FOUND, $"{USER_ID_NO_ORDERS} does not have any orders");
    }

    public static IEnumerable<TestCaseData> CreateOrderTestCases()
    {
        yield return new TestCaseData(validOrder, Statuses.OK, string.Empty);

        yield return new TestCaseData(invalid_order_no_customer, Statuses.INVALID, "Customer is Required");
        yield return new TestCaseData(invalid_order_no_payment, Statuses.INVALID, "PaymentMethod is Required");
        yield return new TestCaseData(invalid_order_no_items, Statuses.INVALID, "Items are Required and can't be empty");

        yield return new TestCaseData(invalid_order_no_product_exist, Statuses.NOT_FOUND, $"Product {PRODUCT_ID_NOT_FOUND} not found");
        yield return new TestCaseData(invalid_order_not_in_stock, Statuses.INVALID, $"Product {PRODUCT_ID_NOT_ON_STOCK} is out of stock");
        yield return new TestCaseData(invalid_order_user_orders_own_product, Statuses.INVALID, $"User {USER_ID_EXISTS} can't order his own product");
    }
}
