using Domain;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class ProductsService : IProductsService
{
    private const string Queue = "products_queue";
    private readonly ILogger<IProductsService> _logger;
    private readonly IProductsRepository _productsRepository;
    private readonly IUsersRepository _userRepository;
    private readonly IMessageProducer _messageProducer;

    public ProductsService(ILogger<IProductsService> logger, IProductsRepository productsRepository, IUsersRepository userRepository, IMessageProducer messageProducer)
    {
        _logger = logger;
        _productsRepository = productsRepository;
        _userRepository = userRepository;
        _messageProducer = messageProducer;
    }

    public Product GetProductById(string id, out Statuses status, out string error)
    {
        _logger.LogInformation("Get Product by id {id}", id);
        error = string.Empty;
        status = Statuses.OK;
        
        var product = _productsRepository.GetProduct(id);

        if (product == null)
        {
            _logger.LogError("Get Product by id {id} not found", id);
            status = Statuses.NOT_FOUND;
            error = $"Product {id} not found";
            return null;
        }
        
        return product;
    }

    public List<Product> GetProducts(List<string> ids, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        
        var products = _productsRepository.GetProducts(ids);

        if (products == null)
        {
            _logger.LogError("Get Products by ids {ids} not found", ids);
            status = Statuses.NOT_FOUND;
            error = $"Products {ids} not found";
            return null;
        }

        return products;
    }

    public List<Product> GetAllProducts(out Statuses status, out string error)
    {
        _logger.LogInformation("Get all products");
        
        var products = _productsRepository.GetAllProducts();

        if (products.Count == 0)
        {
            status = Statuses.NOT_FOUND;
            error = "The store does not have any products";
        }
        else
        {
            var activeUsersIds = _userRepository.GetAllUsers().Where(x => x.IsActive).Select(x => x.UserId).ToList();
            
            products = products.Where(x => activeUsersIds.Contains(x.OwnerId)).ToList();
            
            status = Statuses.OK;
            error = string.Empty;
        }

        return products;
    }

    public List<string> GetAllCategories()
    {
        var categories = _productsRepository.GetAllProducts().DistinctBy(p => p.Category).Select(p => p.Category).ToList();
        return categories;
    }

    public Product UpdateProduct(Product product, out Statuses status, out string error)
    {
        _logger.LogInformation("Updating products {id}", product.ProductId);
        error = string.Empty;
        status = Statuses.OK;
        
        var check = _productsRepository.GetProduct(product.ProductId);

        if (check == null)
        {
            _logger.LogError("Product does not exist {id}",  product.ProductId);
            status = Statuses.NOT_FOUND;
            error = $"Product {product.ProductId} not found";
            return null;
        }
        
        if (isProductValid(product, out error))
        {
            _productsRepository.UpdateProduct(product);
            
            var updatedProduct = _productsRepository.GetProduct(product.ProductId);
            return updatedProduct!;
        }
        
        _logger.LogError("Product is not valid. Error {error}", error);
        status = Statuses.INVALID;
        return null;
    }

    public void DeleteProduct(string id, out Statuses status, out string error)
    {
        _logger.LogInformation("Deleting products {id}", id);
        status = Statuses.OK;
        error = string.Empty;
        
        var product = _productsRepository.GetProduct(id);

        if (product == null)
        {
            _logger.LogError("Product does not exist {id}", id);
            status = Statuses.NOT_FOUND;
            error = $"Product {id} not found";
        }
        else
        {
            _productsRepository.DeleteProduct(product);
        }
    }

    public Product CreateProduct(Product product, out Statuses status, out string error)
    {
        _logger.LogInformation("Creating product");
        error = string.Empty;
        status = Statuses.OK;

        if (isProductValid(product, out error))
        {
            _logger.LogInformation("Product is valid, let's create it");
            Guid productId = Guid.NewGuid();
            product.ProductId = productId.ToString();
            
            _productsRepository.CreateProduct(product);
            var newProduct = _productsRepository.GetProduct(productId.ToString());
            
            _logger.LogInformation("Product is created {id}", productId);
            _messageProducer.PublishMessage(newProduct, Queue);
            return newProduct!;
        }
        
        _logger.LogError("Product is not valid. Error {error}", error);
        status = Statuses.INVALID;
        return null;
    }

    public List<Product> GetProductsByCategory(string category, out Statuses status, out string error)
    {
        _logger.LogInformation("Getting products by category {category}", category);
        error = string.Empty;
        status = Statuses.OK;
        
        var products = _productsRepository.GetProductsByCategory(category);

        if (products.Count == 0)
        {
            _logger.LogError("Category {category} does not have any products",  category);
            status = Statuses.NOT_FOUND;
            error = $"Products from {category} not found";
        }

        return products;
    }

    public List<Product> GetProductsByUser(string user, out Statuses status, out string error)
    {
        _logger.LogInformation("Getting products by user {user}", user);
        error = string.Empty;
        status = Statuses.OK;
        List<Product> products = new List<Product>();
        
        var check = _userRepository.GetUserById(user);

        if (check == null)
        {
            _logger.LogError("User {user} does not exist", user);
            status = Statuses.NOT_FOUND;
            error = $"User {user} not found";
            
        }
        else if (!check.IsActive)
        {
            _logger.LogInformation($"User {user} is not active");
            status = Statuses.INVALID;
            error = $"User {user} is not active";
        }
        else
        {
            products =  _productsRepository.GetProductsByUser(user);

            if (products.Count == 0)
            {
                _logger.LogError("User {user} does not have any products", user);
                status = Statuses.NOT_FOUND;
                error = $"Products by {user} not found";
            }
        }
        
        return products;
    }

    private bool isProductValid(Product product, out string error)
    {
        error = string.Empty;
        if (product.Name == null || product.Name == "")
        {
            error = "Product name is required";
            return false;
        }
        
        if (product.Category == null || product.Category == "")
        {
            error = "Category is required";
            return false;
        }

        if (product.Quantity <= 0)
        {
            error = "Quantity is required and must be greater than 0";
            return false;
        }

        if (product.OwnerId == null || product.OwnerId == string.Empty)
        {
            error = "OwnerId is required";
            return false;
        }
        else
        {
            var owner = _userRepository.GetUserById(product.OwnerId);

            if (owner == null)
            {
                error = $"user {product.OwnerId} does not exist";
                return false;
            }

            if (!owner.IsActive)
            {
                error = $"Inactive users can't add new products to the store";
                return false;
            }
        }

        return true;
    }
}