using Domain;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;
    private readonly IUserRepository _userRepository;

    public ProductsService(IProductsRepository productsRepository, IUserRepository userRepository)
    {
        _productsRepository = productsRepository;
        _userRepository = userRepository;
    }

    public Product GetProductById(string id, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        
        var product = _productsRepository.GetProduct(id);

        if (product == null)
        {
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
            status = Statuses.NOT_FOUND;
            error = $"Products {ids} not found";
            return null;
        }

        return products;
    }

    public Product UpdateProduct(Product product, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        
        var check = _productsRepository.GetProduct(product.ProductId);

        if (check == null)
        {
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
        else
        {
            status = Statuses.INVALID;
            return null;
        }
    }

    public void DeleteProduct(string id, out Statuses status, out string error)
    {
        status = Statuses.OK;
        error = string.Empty;
        
        var product = _productsRepository.GetProduct(id);

        if (product == null)
        {
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
        error = string.Empty;
        status = Statuses.OK;

        if (isProductValid(product, out error))
        {
            Guid productId = Guid.NewGuid();
            product.ProductId = productId.ToString();
            
            _productsRepository.CreateProduct(product);
            var newProduct = _productsRepository.GetProduct(productId.ToString());
            
            return newProduct!;
        }
        else
        {
            status = Statuses.INVALID;
            return null;
        }
    }

    public List<Product> GetProductsByCategory(string category, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        
        var products = _productsRepository.GetProductsByCategory(category);

        if (products.Count == 0)
        {
            status = Statuses.NOT_FOUND;
            error = $"Products from {category} not found";
            return null;
        }

        return products;
    }

    public List<Product> GetProductsByUser(string user, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        
        var check = _userRepository.GetUser(user);

        if (check == null)
        {
            status = Statuses.INVALID;
            error = $"User {user} not found";
            return null;
        }
        
        var products =  _productsRepository.GetProductsByUser(user);

        if (products.Count == 0)
        {
            status = Statuses.NOT_FOUND;
            error = $"Products by {user} not found";
        }

        return products;
    }

    private bool isProductValid(Product product, out string error)
    {
        error = string.Empty;
        if (product.Category == null || product.Category == "")
        {
            error += "Category is required";
        }

        if (product.Name == null || product.Name == "")
        {
            error += " Product name is required";
        }

        if (product.OwnerId == null || product.OwnerId == "")
        {
            error += " OwnerId is required";
        }

        if (product.Quantity <= 0)
        {
            error += " Quantity is required and must be greater than 0";
        }

        if (product.OwnerId == null || product.OwnerId == string.Empty)
        {
            error += " OwnerId is required";
        }
        else
        {
            var owner = _userRepository.GetUser(product.OwnerId);

            if (owner == null)
            {
                error += $" user {product.OwnerId} does not exist";
            }
        }

        return error == string.Empty;
    }
}