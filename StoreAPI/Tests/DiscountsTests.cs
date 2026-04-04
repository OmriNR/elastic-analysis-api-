using Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework.Internal;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Services;

namespace TestProject1;

public class DiscountsTests
{
    private const string DISCOUT_ID_EXISTS = "1";
    private const string DISCOUT_ID_NOT_EXISTS = "2";
    
    private const string PRODUCT_ID_NOT_EXISTS = "3";
    private const string PRODUCT_ID_ON_DISCOUT = "4";
    private const string PRODUCT_ID_NOT_ON_DISCOUT = "5";

    private const string EXISTS_USER = "6";
    private const string EXISTS_USER_NO_PRODUCTS = "7";
    private const string NOT_EXISTS_USER = "9";
    
    private const string EXISTS_CATEGORY = "CATEGORY";
    private const string NOT_EXISTS_CATEGORY = "CATEGORY_NOT_EXISTS";

    private const double NOT_VALID_PERCANTAGE = 101;
    private const double VALID_PERCENTAGE = 99;

    private static readonly DateTime EXPIRED_DATE = DateTime.MinValue;
    private static readonly DateTime NOT_EXPIRED_DATE = DateTime.MaxValue;

    private User _validUser = new User()
    {
        UserId = EXISTS_USER
    };
    
    private User _emptyUser = new User()
    {
        UserId = EXISTS_USER_NO_PRODUCTS
    };
    
    private Product product_on_discount = new Product()
    {
        ProductId = PRODUCT_ID_ON_DISCOUT,
        Category = EXISTS_CATEGORY,
        OwnerId = EXISTS_USER
    };

    private Product product_not_on_discount = new Product()
    {
        ProductId = PRODUCT_ID_NOT_ON_DISCOUT,
        Category = EXISTS_CATEGORY,
        OwnerId = EXISTS_USER
    };

    private static Discount valid_discount = new Discount()
    {
        DiscountId = DISCOUT_ID_EXISTS,
        Percentage = VALID_PERCENTAGE,
        ExpiredAt = NOT_EXPIRED_DATE,
        ProdcutId = PRODUCT_ID_NOT_ON_DISCOUT
    };
    
    private static Discount invalid_discount_product_not_exist = new Discount()
    {
        DiscountId = DISCOUT_ID_EXISTS,
        Percentage = VALID_PERCENTAGE,
        ExpiredAt = NOT_EXPIRED_DATE,
        ProdcutId = PRODUCT_ID_NOT_EXISTS 
    };

    private static Discount invalid_discount_percentage = new Discount()
    {
        DiscountId = DISCOUT_ID_EXISTS,
        Percentage = NOT_VALID_PERCANTAGE
    };

    private static Discount invalid_discount_expired = new Discount()
    {
        DiscountId = DISCOUT_ID_EXISTS,
        Percentage = VALID_PERCENTAGE,
        ExpiredAt = EXPIRED_DATE
    };

    private static Discount invalid_discount_empty = new Discount()
    {
        DiscountId = DISCOUT_ID_EXISTS,
        Percentage = VALID_PERCENTAGE,
        ExpiredAt = NOT_EXPIRED_DATE,
        ProdcutId = ""
    };
    
    private Mock<IDiscountsRepository> _discountsRepositoryMock;
    private Mock<IProductsRepository> _productsRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private IDiscountsService _service;
    
    [SetUp]
    public void Setup()
    {
        _discountsRepositoryMock = new Mock<IDiscountsRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _productsRepositoryMock = new Mock<IProductsRepository>();

        _discountsRepositoryMock.Setup(repo =>
            repo.GetDiscount(It.Is<string>(p => p != DISCOUT_ID_NOT_EXISTS))).Returns(valid_discount);
        
        _discountsRepositoryMock.Setup(repo =>
            repo.GetDiscount(It.Is<string>(p => p == DISCOUT_ID_NOT_EXISTS))).Returns((Discount)null);
        
        _discountsRepositoryMock.Setup(repo => 
            repo.GetDiscountByProduct(It.Is<string>(p => p == PRODUCT_ID_ON_DISCOUT))).Returns(valid_discount);
        
        _discountsRepositoryMock.Setup(repo => 
            repo.GetDiscountByProduct(It.Is<string>(p => p != PRODUCT_ID_ON_DISCOUT))).Returns((Discount)null);

        _productsRepositoryMock.Setup(repo => 
            repo.GetProduct(It.Is<string>(p => p == PRODUCT_ID_NOT_EXISTS))).Returns((Product)null);
        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(It.Is<string>(p => p == PRODUCT_ID_ON_DISCOUT))).Returns(product_on_discount);
        
        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(It.Is<string>(p => p == PRODUCT_ID_NOT_ON_DISCOUT))).Returns(product_not_on_discount);
        
        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(It.Is<string>(p => p == PRODUCT_ID_NOT_ON_DISCOUT))).Returns(product_not_on_discount);
        
        _productsRepositoryMock.Setup(repo => 
            repo.GetProductsByUser(EXISTS_USER_NO_PRODUCTS)).Returns(new List<Product>());
        
        _productsRepositoryMock.Setup(repo => 
            repo.GetProductsByUser(EXISTS_USER)).Returns(new List<Product>() { product_not_on_discount });
        
        _productsRepositoryMock.Setup(repo =>
            repo.GetProductsByCategory(EXISTS_CATEGORY)).Returns(new List<Product>(){ product_not_on_discount });
        
        _productsRepositoryMock.Setup(repo =>
            repo.GetProductsByCategory(NOT_EXISTS_CATEGORY)).Returns(new List<Product>());
        
        _userRepositoryMock.Setup(repo =>
            repo.GetUser(It.Is<string>(id => id == EXISTS_USER))).Returns(_validUser);
        
        _userRepositoryMock.Setup(repo =>
            repo.GetUser(It.Is<string>(id => id == EXISTS_USER_NO_PRODUCTS))).Returns(_emptyUser);
        
        _userRepositoryMock.Setup(repo =>
            repo.GetUser(It.Is<string>(id => id == NOT_EXISTS_USER))).Returns((User)null);

        ILogger<IDiscountsService> _logger = new NullLogger<IDiscountsService>();
        _service = new DiscountService(_logger, _discountsRepositoryMock.Object, _userRepositoryMock.Object, _productsRepositoryMock.Object);
    }

    [TestCase(DISCOUT_ID_EXISTS, Statuses.OK, "")]
    [TestCase(DISCOUT_ID_NOT_EXISTS, Statuses.NOT_FOUND, $"Discount {DISCOUT_ID_NOT_EXISTS} not found")]
    public void Get_discount_by_id(string id, Statuses expectedStatus, string expectedError)
    {
        var discount = _service.GetDiscountById(id, out var status, out var error);
        
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [TestCase(PRODUCT_ID_ON_DISCOUT, Statuses.OK, "")]
    [TestCase(PRODUCT_ID_NOT_ON_DISCOUT, Statuses.NOT_FOUND, $"Active discount of product {PRODUCT_ID_NOT_ON_DISCOUT} not found")]
    [TestCase(PRODUCT_ID_NOT_EXISTS,  Statuses.NOT_FOUND, $"Product {PRODUCT_ID_NOT_EXISTS} not found")]
    public void Get_discount_by_product(string productId, Statuses expectedStatus, string expectedError)
    {
        var discount = _service.GetDiscountByProduct(productId, out var status, out var error);
        
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }
    
    [Test, TestCaseSource(nameof(CreateDiscountTestCases))]
    public void Create_discount(Discount discount, Statuses expectedStatus, string expectedError)
    {
        _service.CreateDiscount(discount, out var status, out var error);
        
        Assert.That(status,  Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [Test, TestCaseSource(nameof(CreateDiscountCategoryTestCases))]
    public void Create_discount_by_category(Discount discount, string category, Statuses expectedStatus, string expectedError)
    {
        _service.CreateDiscountsByCategory(discount, category, out var status, out var error);
        
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    [Test, TestCaseSource(nameof(CreateDiscountUserTestCases))]
    public void Create_discount_user(Discount discount, string user, Statuses expectedStatus, string expectedError)
    {
        _service.CreateDiscountsByUser(discount, user, out var status, out var error);
        
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error, Is.EqualTo(expectedError));
    }

    public static IEnumerable<TestCaseData> CreateDiscountTestCases()
    {
        yield return new TestCaseData(
            valid_discount,
            Statuses.OK,
            ""
        );

        yield return new TestCaseData(
            invalid_discount_percentage,
            Statuses.INVALID,
            "Discount percentage must be between 0 and 100");
        
        yield return new TestCaseData(
            invalid_discount_expired,
            Statuses.INVALID,
            "Can't create expired discount");
        
        yield return new TestCaseData(
            invalid_discount_product_not_exist,
            Statuses.INVALID,
            "Discount product must be non-null");
    }

    public static IEnumerable<TestCaseData> CreateDiscountCategoryTestCases()
    {
        yield return new TestCaseData(
            valid_discount,
            EXISTS_CATEGORY,
            Statuses.OK,
            "");
        
        yield return new TestCaseData(
            valid_discount,
            NOT_EXISTS_CATEGORY,
            Statuses.NOT_FOUND,
            $"Category {NOT_EXISTS_CATEGORY} not found");
        
        yield return new TestCaseData(
            invalid_discount_percentage,
            EXISTS_CATEGORY,
            Statuses.INVALID,
            "Discount percentage must be between 0 and 100");
        
        yield return new TestCaseData(
            invalid_discount_expired,
            EXISTS_CATEGORY,
            Statuses.INVALID,
            "Can't create expired discount");
    }
    
    public static IEnumerable<TestCaseData> CreateDiscountUserTestCases()
    {
        yield return new TestCaseData(
            valid_discount,
            EXISTS_USER,
            Statuses.OK,
            "");
        
        yield return new TestCaseData(
            valid_discount,
            NOT_EXISTS_USER,
            Statuses.NOT_FOUND,
            $"User {NOT_EXISTS_USER} not found");

        yield return new TestCaseData(
            valid_discount,
            EXISTS_USER_NO_PRODUCTS,
            Statuses.NOT_FOUND,
            $"User {EXISTS_USER_NO_PRODUCTS} does not have any products");
        
        yield return new TestCaseData(
            invalid_discount_percentage,
            EXISTS_USER,
            Statuses.INVALID,
            "Discount percentage must be between 0 and 100");
        
        yield return new TestCaseData(
            invalid_discount_expired,
            EXISTS_USER,
            Statuses.INVALID,
            "Can't create expired discount");
    }
}