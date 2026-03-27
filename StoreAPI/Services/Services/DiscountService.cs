using Domain;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class DiscountService : IDiscountsService
{
    private readonly ILogger<IDiscountsService> _logger;
    private readonly IDiscountsRepository _discountsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductsRepository _productsRepository;

    public DiscountService(ILogger<IDiscountsService> logger, IDiscountsRepository discountsRepository, IUserRepository userRepository, IProductsRepository productsRepository)
    {
        _logger = logger;
        _discountsRepository = discountsRepository;
        _userRepository = userRepository;
        _productsRepository = productsRepository;
    }

    public Discount GetDiscountById(string id, out Statuses status, out string error)
    {
        _logger.LogInformation($"Getting discount by {id}", DateTime.Now);
        error = string.Empty;
        status = Statuses.OK;

        var discount = _discountsRepository.GetDiscount(id);

        if (discount == null)
        {
            _logger.LogError($"Discount {id} not found");
            status = Statuses.NOT_FOUND;
            error = $"Discount {id} not found";

            return null;
        }
        
        return discount;
    }

    public Discount GetDiscountByProduct(string productId, out Statuses status, out string error)
    {
        _logger.LogInformation($"Getting discount by product {productId}", DateTime.Now);
        error = string.Empty;
        status = Statuses.OK;
        
        var product = _productsRepository.GetProduct(productId);
        if (product == null)
        {
            _logger.LogError($"Product {productId} not found");
            status = Statuses.NOT_FOUND;
            error = $"Product {productId} not found";
            return null;
        }

        var discount = _discountsRepository.GetDiscountByProduct(productId);

        if (discount == null)
        {
            _logger.LogError($"Porudct {productId} doesn't have active discounts");
            status = Statuses.NOT_FOUND;
            error = $"Active discount of product {productId} not found";
            return null;
        }
        
        return discount;
    }

    public Discount CreateDiscount(Discount discount, out Statuses status, out string error)
    {
        _logger.LogInformation($"Creating discount {discount}", DateTime.Now);
        error = string.Empty;
        status = Statuses.OK;

        if (IsDiscountValid(discount, out error, out var productsNotOnDiscount))
        {
            _logger.LogInformation($"{discount} is valid, let's create it");
            Guid discountId = Guid.NewGuid();
            discount.DiscountId = discountId.ToString();
            discount.Products =  productsNotOnDiscount;
            
            _discountsRepository.CreateDiscount(discount);
            var newDiscount = _discountsRepository.GetDiscount(discount.DiscountId);

            _logger.LogInformation($"Discount created successfully, new Discount Id: {newDiscount!.DiscountId}");
            return newDiscount!;
        }
        
        _logger.LogError($"Discount {discount} is not valid. Error: {error}");
        status = Statuses.INVALID;
        return null;
    }

    public Discount CreateDiscountByCategory(Discount discount, string category,  out Statuses status, out string error)
    {
        _logger.LogInformation($"Creating a discount for all products of category {category}",  DateTime.Now);
        error = string.Empty;
        status =  Statuses.OK;
        
        var productsByCategory =  _productsRepository.GetProductsByCategory(category).Select(p => p.ProductId).ToList();

        if (productsByCategory.Count == 0)
        {
            _logger.LogError($"Category {category} not found");
            status = Statuses.NOT_FOUND;
            error =  $"Category {category} not found";
            return null;
        }
        
        discount.Products = productsByCategory;

        if (IsDiscountValid(discount, out error, out var productsNotOnDiscount))
        {
            _logger.LogInformation($"{discount} is valid, let's create it");
            string discountId = Guid.NewGuid().ToString();
            discount.DiscountId = discountId;
            discount.Products =  productsNotOnDiscount;
            
            _discountsRepository.CreateDiscount(discount);
            var newDiscount = _discountsRepository.GetDiscount(discountId);

            _logger.LogInformation($"Discount created successfully, new Discount Id: {newDiscount!.DiscountId}");
            return newDiscount!;
        }
        
        _logger.LogError($"Discount {discount} is not valid. Error: {error}");
        status = Statuses.INVALID;
        return null;
    }

    public Discount CreateDiscountByUser(Discount discount, string userId, out Statuses status, out string error)
    {
        _logger.LogInformation($"Creating a discount for all products of user {userId}",  DateTime.Now);
        error = string.Empty;
        status = Statuses.OK;
        
        var user = _userRepository.GetUser(userId);

        if (user == null)
        {
            _logger.LogError($"User {userId} not found");
            status = Statuses.NOT_FOUND;
            error = $"User {userId} not found";
            return null;
        }
        
        var productsByUser = _productsRepository.GetProductsByUser(userId).Select(p => p.ProductId).ToList();

        if (productsByUser.Count == 0)
        {
            _logger.LogError($"User {userId} does not have any products");
            status = Statuses.NOT_FOUND;
            error =  $"User {userId} does not have any products";
            return null;
        }
        
        discount.Products = productsByUser;

        if (IsDiscountValid(discount, out error, out var productsNotOnDiscount))
        {
            _logger.LogInformation($"{discount} is valid, let's create it");
            string discountId = Guid.NewGuid().ToString();
            discount.DiscountId = discountId;
            discount.Products =  productsNotOnDiscount;
            
            _discountsRepository.CreateDiscount(discount);
            var newDiscount = _discountsRepository.GetDiscount(discountId);
            
            _logger.LogInformation($"Discount created successfully, new Discount Id: {newDiscount!.DiscountId}");
            return newDiscount!;
        }
        
        _logger.LogError($"Discount {discount} is not valid. Error: {error}");
        status = Statuses.INVALID;
        return null;
    }

    /*public Discount UpdateDiscount(Discount discount, out Statuses status, out string error)
    {
        error = string.Empty;
        status = Statuses.OK;
        
        var check = _discountsRepository.GetDiscount(discount.DiscountId);

        if (check == null)
        {
            status = Statuses.NOT_FOUND;
            error = $"Discount {discount.DiscountId} not found";
            return null;
        }

        if (IsDiscountValid(discount, out error,  out var productsNotOnDiscount))
        {
            _discountsRepository.UpdateDiscount(discount);
            
            var updatedDiscount = _discountsRepository.GetDiscount(discount.DiscountId);
            return updatedDiscount!;
        }
        
        status = Statuses.INVALID;
        return null;
    }*/

    private bool IsDiscountValid(Discount discount, out string error, out List<string> productsNotOnDiscount)
    {
        error = string.Empty;
        productsNotOnDiscount = new List<string>();
        
        if (discount.Percentage <= 0 || discount.Percentage >= 100)
        {
            error = "Discount percentage must be between 0 and 100";
            return false;
        }
        
        if (discount.ExpiredAt <= DateTime.Now)
        {
            error = "Can't create expired discount";
            return false;
        }
        
        if (discount.Products.Count == 0)
        {
            error = "Discount must connect to products";
            return false;
        }
        
        if (discount.Products.Any(p => _productsRepository.GetProduct(p) == null))
        {
            error = "Discount product must be non-null";
            return false;
        }
        
        productsNotOnDiscount = discount.Products.Where(p => _discountsRepository.GetDiscountByProduct(p) == null).ToList();

        if (productsNotOnDiscount.Count == 0)
        {
            error = "All products are already on discount";
            return false;
        }

        return true;
    }
}