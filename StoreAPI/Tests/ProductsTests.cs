using Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Services;

namespace TestProject1;

public class ProductsTests
{
    private const string PRODUCT_ID_EXIST = "1";
    private const string PRODUCT_ID_NOT_FOUND = "2";

    private const string USER_ID_EXIST = "3";
    private const string USER_ID_NOT_FOUND = "4";
    private const string USER_ID_INACTIVE = "5";

    private const string PRODUCT_NAME = "Product";
    private const string NO_FOUND_PRODUCT_CATEGORY = "NoCategory";
    private const string PRODUCT_CATEGORY = "Category";
    private const int PRODUCT_QUANTITY = 1;

    private static User activeUser = new User() { UserId = USER_ID_EXIST, IsActive = true };
    private static User inactiveUser = new User() { UserId = USER_ID_INACTIVE, IsActive = false };

    private static Product validProduct = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = PRODUCT_CATEGORY,
        SubCategory = "TestSubCategory",
        Description = "TestDescription",
        OwnerId = USER_ID_EXIST,
        Price = 100,
        Quantity = PRODUCT_QUANTITY
    };

    private static Product nonExistsProduct = new Product()
    {
        ProductId = PRODUCT_ID_NOT_FOUND
    };

    private static Product productNoName = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = ""
    };

    private static Product productNoCategory = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = ""
    };

    private static Product productNoQuantity = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = PRODUCT_CATEGORY,
        Quantity = 0
    };

    private static Product productNoOwner = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = PRODUCT_CATEGORY,
        Quantity = PRODUCT_QUANTITY,
        OwnerId = ""
    };

    private static Product productOwnerNoExist = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = PRODUCT_CATEGORY,
        Quantity = PRODUCT_QUANTITY,
        OwnerId = USER_ID_NOT_FOUND
    };

    private static Product productOwnerInactive = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = PRODUCT_CATEGORY,
        Quantity = PRODUCT_QUANTITY,
        OwnerId = USER_ID_INACTIVE
    };

    private Mock<IProductsRepository> _productsRepositoryMock;
    private Mock<IUsersRepository> _userRepositoryMock;
    private Mock<IMessageProducer> _messageProducerMock;
    private IProductsService _service;

    [SetUp]
    public void Setup()
    {
        _productsRepositoryMock = new Mock<IProductsRepository>();
        _userRepositoryMock = new Mock<IUsersRepository>();
        _messageProducerMock = new Mock<IMessageProducer>();

        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(It.Is<string>(id => id != PRODUCT_ID_NOT_FOUND))).Returns(validProduct);

        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(It.Is<string>(id => id == PRODUCT_ID_NOT_FOUND))).Returns((Product)null);

        _productsRepositoryMock.Setup(repo =>
            repo.GetProductsByCategory(It.Is<string>(c => c == PRODUCT_CATEGORY))).Returns(new List<Product> { validProduct });

        _productsRepositoryMock.Setup(repo =>
            repo.GetProductsByCategory(It.Is<string>(c => c == NO_FOUND_PRODUCT_CATEGORY))).Returns(new List<Product>());

        _productsRepositoryMock.Setup(repo =>
            repo.GetProductsByUser(It.Is<string>(id => id == USER_ID_EXIST))).Returns(new List<Product>() { validProduct });

        _productsRepositoryMock.Setup(repo =>
            repo.GetProductsByUser(It.Is<string>(id => id != USER_ID_EXIST))).Returns(new List<Product>());

        _productsRepositoryMock.Setup(repo =>
            repo.GetAllProducts()).Returns(new List<Product> { validProduct });

        _productsRepositoryMock.Setup(repo =>
            repo.GetProducts(It.Is<List<string>>(ids => ids.Contains(PRODUCT_ID_EXIST)))).Returns(new List<Product> { validProduct });

        _productsRepositoryMock.Setup(repo =>
            repo.GetProducts(It.Is<List<string>>(ids => !ids.Contains(PRODUCT_ID_EXIST)))).Returns((List<Product>)null);

        _userRepositoryMock.Setup(repo => repo.GetUserById(It.Is<string>(id => id == USER_ID_EXIST))).Returns(activeUser);
        _userRepositoryMock.Setup(repo => repo.GetUserById(It.Is<string>(id => id == USER_ID_NOT_FOUND))).Returns((User)null);
        _userRepositoryMock.Setup(repo => repo.GetUserById(It.Is<string>(id => id == USER_ID_INACTIVE))).Returns(inactiveUser);
        _userRepositoryMock.Setup(repo => repo.GetAllUsers()).Returns(new List<User> { activeUser });

        ILogger<IProductsService> logger = new NullLogger<IProductsService>();
        _service = new ProductsService(logger, _productsRepositoryMock.Object, _userRepositoryMock.Object, _messageProducerMock.Object);
    }

    [Test]
    public void Get_By_Id_success()
    {
        var product = _service.GetProductById(PRODUCT_ID_EXIST, out var status, out var error);

        Assert.That(product, Is.Not.Null);
        Assert.That(status, Is.EqualTo(Statuses.OK));
        Assert.That(error, Is.EqualTo(""));
    }

    [Test]
    public void Get_By_Id_not_found()
    {
        var product = _service.GetProductById(PRODUCT_ID_NOT_FOUND, out var status, out var error);

        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(Statuses.NOT_FOUND));
        Assert.That(error, Is.EqualTo($"Product {PRODUCT_ID_NOT_FOUND} not found"));
    }

    [Test]
    public void Get_All_Products_success()
    {
        var products = _service.GetAllProducts(out var status, out var error);

        Assert.That(products.Count, Is.GreaterThan(0));
        Assert.That(status, Is.EqualTo(Statuses.OK));
        Assert.That(error, Is.EqualTo(""));
    }

    [Test]
    public void Get_All_Products_empty_store()
    {
        _productsRepositoryMock.Setup(repo => repo.GetAllProducts()).Returns(new List<Product>());

        var products = _service.GetAllProducts(out var status, out var error);

        Assert.That(status, Is.EqualTo(Statuses.NOT_FOUND));
        Assert.That(error, Is.EqualTo("The store does not have any products"));
    }

    [Test]
    public void Get_All_Products_filters_products_of_inactive_owners()
    {
        var inactiveOwnerProduct = new Product() { ProductId = "other", OwnerId = USER_ID_INACTIVE };
        _productsRepositoryMock.Setup(repo => repo.GetAllProducts()).Returns(new List<Product> { inactiveOwnerProduct });

        var products = _service.GetAllProducts(out var status, out var error);

        Assert.That(products.Count, Is.EqualTo(0));
        Assert.That(status, Is.EqualTo(Statuses.OK));
    }

    [Test]
    public void Get_Products_by_ids_success()
    {
        var ids = new List<string> { PRODUCT_ID_EXIST };
        var products = _service.GetProducts(ids, out var status, out var error);

        Assert.That(products, Is.Not.Null);
        Assert.That(products.Count, Is.EqualTo(1));
        Assert.That(status, Is.EqualTo(Statuses.OK));
        Assert.That(error, Is.EqualTo(""));
    }

    [Test]
    public void Get_Products_by_ids_not_found()
    {
        var ids = new List<string> { PRODUCT_ID_NOT_FOUND };
        var products = _service.GetProducts(ids, out var status, out var error);

        Assert.That(products, Is.Null);
        Assert.That(status, Is.EqualTo(Statuses.NOT_FOUND));
    }

    [Test]
    public void Get_All_Categories_returns_distinct_categories()
    {
        var categories = _service.GetAllCategories();

        Assert.That(categories, Is.Not.Null);
        Assert.That(categories, Contains.Item(PRODUCT_CATEGORY));
        Assert.That(categories.Distinct().Count(), Is.EqualTo(categories.Count));
    }

    [Test]
    public void Get_Category_success()
    {
        var products = _service.GetProductsByCategory(PRODUCT_CATEGORY, out var status, out var error);

        Assert.That(products.Count, Is.EqualTo(1));
        Assert.That(status, Is.EqualTo(Statuses.OK));
        Assert.That(error, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Get_Category_not_found()
    {
        var products = _service.GetProductsByCategory(NO_FOUND_PRODUCT_CATEGORY, out var status, out var error);

        Assert.That(products.Count, Is.EqualTo(0));
        Assert.That(status, Is.EqualTo(Statuses.NOT_FOUND));
        Assert.That(error, Is.EqualTo($"Products from {NO_FOUND_PRODUCT_CATEGORY} not found"));
    }

    [Test]
    public void Get_User_success()
    {
        var products = _service.GetProductsByUser(USER_ID_EXIST, out var status, out var error);

        Assert.That(products.Count, Is.EqualTo(1));
        Assert.That(status, Is.EqualTo(Statuses.OK));
        Assert.That(error, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Get_User_not_found()
    {
        var products = _service.GetProductsByUser(USER_ID_NOT_FOUND, out var status, out var error);

        Assert.That(products.Count, Is.EqualTo(0));
        Assert.That(status, Is.EqualTo(Statuses.NOT_FOUND));
        Assert.That(error, Is.EqualTo($"User {USER_ID_NOT_FOUND} not found"));
    }

    [Test]
    public void Get_User_inactive()
    {
        var products = _service.GetProductsByUser(USER_ID_INACTIVE, out var status, out var error);

        Assert.That(status, Is.EqualTo(Statuses.INVALID));
        Assert.That(error, Is.EqualTo($"User {USER_ID_INACTIVE} is not active"));
    }

    [Test]
    public void Delete_Product_success()
    {
        _service.DeleteProduct(PRODUCT_ID_EXIST, out var status, out var error);

        Assert.That(status, Is.EqualTo(Statuses.OK));
        Assert.That(error, Is.EqualTo(""));
        _productsRepositoryMock.Verify(repo => repo.DeleteProduct(It.IsAny<Product>()), Times.Once);
    }

    [Test]
    public void Delete_Product_not_found()
    {
        _service.DeleteProduct(PRODUCT_ID_NOT_FOUND, out var status, out var error);

        Assert.That(status, Is.EqualTo(Statuses.NOT_FOUND));
        Assert.That(error, Is.EqualTo($"Product {PRODUCT_ID_NOT_FOUND} not found"));
        _productsRepositoryMock.Verify(repo => repo.DeleteProduct(It.IsAny<Product>()), Times.Never);
    }

    [Test, TestCaseSource(nameof(CreateProductTestCases))]
    public void Create_Product(Product product, Statuses expectedStatus, string expectedError)
    {
        _service.CreateProduct(product, out var status, out var error);

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [Test, TestCaseSource(nameof(UpdateProductTestCases))]
    public void Update_Product(Product product, Statuses expectedStatus, string expectedError)
    {
        _service.UpdateProduct(product, out var status, out var error);

        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    public static IEnumerable<TestCaseData> CreateProductTestCases()
    {
        yield return new TestCaseData(validProduct, Statuses.OK, "");
        yield return new TestCaseData(productNoName, Statuses.INVALID, "Product name is required");
        yield return new TestCaseData(productNoCategory, Statuses.INVALID, "Category is required");
        yield return new TestCaseData(productNoQuantity, Statuses.INVALID, "Quantity is required and must be greater than 0");
        yield return new TestCaseData(productNoOwner, Statuses.INVALID, "OwnerId is required");
        yield return new TestCaseData(productOwnerNoExist, Statuses.INVALID, $"user {USER_ID_NOT_FOUND} does not exist");
        yield return new TestCaseData(productOwnerInactive, Statuses.INVALID, "Inactive users can't add new products to the store");
    }

    public static IEnumerable<TestCaseData> UpdateProductTestCases()
    {
        yield return new TestCaseData(nonExistsProduct, Statuses.NOT_FOUND, $"Product {PRODUCT_ID_NOT_FOUND} not found");
        foreach (var tc in CreateProductTestCases()) yield return tc;
    }
}
