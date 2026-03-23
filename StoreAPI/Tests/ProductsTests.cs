using Domain;
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
    
    private const string PRODUCT_NAME = "Product";
    private const string NO_FOUND_PRODUCT_CATEGORY = "NoCategory";
    private const string PRODUCT_CATEGORY = "Category";
    private const int PRODUCT_QUANTITY = 1;
    
    private Product validProduct = new Product()
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

    private Product nonExistsProduct = new Product()
    {
        ProductId = PRODUCT_ID_NOT_FOUND
    };

    private Product productNoName = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = ""
    };

    private Product productNoCategory = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = ""
    };

    private Product productNoQuantity = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = PRODUCT_CATEGORY,
        Quantity = 0
    };

    private Product productNoOwner = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = PRODUCT_CATEGORY,
        Quantity = PRODUCT_QUANTITY,
        OwnerId = ""
    };

    private Product productOwnerNoExist = new Product()
    {
        ProductId = PRODUCT_ID_EXIST,
        Name = PRODUCT_NAME,
        Category = PRODUCT_CATEGORY,
        Quantity = PRODUCT_QUANTITY,
        OwnerId = USER_ID_NOT_FOUND
    };
    
    private Mock<IProductsRepository> _productsRepositoryMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private IProductsService _service;
    
    [SetUp]
    public void Setup()
    {
        _productsRepositoryMock = new Mock<IProductsRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        //Set productRepo
        _productsRepositoryMock.Setup(repo =>
            repo.GetProduct(It.Is<string>(id => id != PRODUCT_ID_NOT_FOUND))).Returns(validProduct);
        
        _productsRepositoryMock.Setup(repo => 
            repo.GetProduct(It.Is<string>(id => id == PRODUCT_ID_NOT_FOUND))).Returns((Product)null);
        
        _productsRepositoryMock.Setup(repo => 
            repo.GetProductsByCategory(It.Is<string>(id  => id == PRODUCT_CATEGORY))).Returns(new List<Product> { validProduct});
        
        _productsRepositoryMock.Setup(repo => 
            repo.GetProductsByCategory(It.Is<string>(id  => id == NO_FOUND_PRODUCT_CATEGORY))).Returns(new List<Product>());
        
        _productsRepositoryMock.Setup(repo => 
            repo.GetProductsByUser(It.Is<string>(id => id == USER_ID_EXIST))).Returns(new List<Product>() { validProduct });
        
        _productsRepositoryMock.Setup(repo => 
            repo.GetProductsByUser(It.Is<string>(id => id == USER_ID_NOT_FOUND))).Returns(new List<Product>());
        
        _userRepositoryMock.Setup(repo => repo.GetUser(It.Is<string>(id => id == USER_ID_EXIST))).Returns(new User());
        _userRepositoryMock.Setup(repo => repo.GetUser(It.Is<string>(id => id == USER_ID_NOT_FOUND))).Returns((User)null);
        
        _service = new ProductsService(_productsRepositoryMock.Object,  _userRepositoryMock.Object);

    }

    [Test]
    public void Get_By_Id_success()
    {
        var expectedStatus = Statuses.OK;
        var expectedError = "";

        var product = _service.GetProductById(PRODUCT_ID_EXIST, out var status, out var error);
        
        Assert.That(product, Is.Not.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Get_By_Id_not_found()
    {
        var expectedStatus = Statuses.NOT_FOUND;
        var expectedError = $"Product {PRODUCT_ID_NOT_FOUND} not found";

        var product = _service.GetProductById(PRODUCT_ID_NOT_FOUND, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }

    [Test]
    public void Get_Category_success()
    {
        var expectedStatus = Statuses.OK;
        var expectedError = string.Empty;

        var products = _service.GetProductsByCategory(PRODUCT_CATEGORY, out var status, out var error);
        
        Assert.That(products.Count, Is.EqualTo(1));
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }

    [Test]
    public void Get_Category_not_found()
    {
        var expectedStatus = Statuses.NOT_FOUND;
        var expectedError = $"Products from {NO_FOUND_PRODUCT_CATEGORY} not found";

        var products = _service.GetProductsByCategory(NO_FOUND_PRODUCT_CATEGORY, out var status, out var error);
        
        Assert.That(products.Count, Is.EqualTo(0));
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Get_User_success()
    {
        var expectedStatus = Statuses.OK;
        var expectedError = string.Empty;

        var products = _service.GetProductsByUser(USER_ID_EXIST, out var status, out var error);
        
        Assert.That(products.Count, Is.EqualTo(1));
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }

    [Test]
    public void Get_User_not_found()
    {
        var expectedStatus = Statuses.NOT_FOUND;
        var expectedError = $"User {USER_ID_NOT_FOUND} not found";
        
        var products = _service.GetProductsByUser(USER_ID_NOT_FOUND, out var status, out var error);
        
        Assert.That(products.Count, Is.EqualTo(0));
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Update_Product_not_found()
    {
        var expectedStatus = Statuses.NOT_FOUND;
        var expectedError =  $"Product {PRODUCT_ID_NOT_FOUND} not found";
        
        var product = _service.UpdateProduct(nonExistsProduct, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Update_Product_name_not_valid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  "Product name is required";
        
        var product = _service.UpdateProduct(productNoName, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Update_Product_category_not_valid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  "Category is required";
        
        var product = _service.UpdateProduct(productNoCategory, out var status, out var error);
        
        Assert.That(product,  Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Update_Product_quantity_not_valid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  "Quantity is required and must be greater than 0";
        
        var product = _service.UpdateProduct(productNoQuantity, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Update_Product_owner_not_valid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  "OwnerId is required";
        
        var product = _service.UpdateProduct(productNoOwner, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Update_Product_owner_not_exist()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  $"user {USER_ID_NOT_FOUND} does not exist";
        
        var product = _service.UpdateProduct(productOwnerNoExist, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Update_success()
    {
        var expectedStatus = Statuses.OK;
        var expectedError = string.Empty;
        
        var product = _service.UpdateProduct(validProduct, out var status, out var error);
        
        Assert.That(product, Is.Not.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Create_Product_name_not_valid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  "Product name is required";
        
        var product = _service.CreateProduct(productNoName, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Create_Product_category_not_valid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  "Category is required";
        
        var product = _service.CreateProduct(productNoCategory, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Create_Product_quantity_not_valid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  "Quantity is required and must be greater than 0";
        
        var product = _service.CreateProduct(productNoQuantity, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Create_Product_owner_not_valid()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  "OwnerId is required";
        
        var product = _service.CreateProduct(productNoOwner, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Create_Product_owner_not_exist()
    {
        var expectedStatus = Statuses.INVALID;
        var expectedError =  $"user {USER_ID_NOT_FOUND} does not exist";
        
        var product = _service.CreateProduct(productOwnerNoExist, out var status, out var error);
        
        Assert.That(product, Is.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));
    }
    
    [Test]
    public void Create_success()
    {
        var expectedStatus = Statuses.OK;
        var expectedError = string.Empty;
        
        var product = _service.CreateProduct(validProduct, out var status, out var error);
        
        Assert.That(product, Is.Not.Null);
        Assert.That(status, Is.EqualTo(expectedStatus));
        Assert.That(error,  Is.EqualTo(expectedError));;
    }
}