using Domain;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services;

public class DiscountService : IDiscountsService
{
    private readonly ILogger<IDiscountsService> _logger;
    private readonly IDiscountsRepository _discountsRepository;
    private readonly IUsersPropertiesRepository _userRepository;
    private readonly IProductsRepository _productsRepository;

    public DiscountService(ILogger<IDiscountsService> logger, IDiscountsRepository discountsRepository, IUsersPropertiesRepository userRepository, IProductsRepository productsRepository)
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

        if (IsDiscountValid(discount, out error))
        {
            _logger.LogInformation($"{discount} is valid, let's create it");
            Guid discountId = Guid.NewGuid();
            discount.DiscountId = discountId.ToString();
            
            _discountsRepository.CreateDiscount(discount);
            var newDiscount = _discountsRepository.GetDiscount(discount.DiscountId);

            _logger.LogInformation($"Discount created successfully, new Discount Id: {newDiscount!.DiscountId}");
            return newDiscount!;
        }
        
        _logger.LogError($"Discount {discount} is not valid. Error: {error}");
        status = Statuses.INVALID;
        return null;
    }

    public List<Discount> CreateDiscountsByCategory(Discount discount, string category,  out Statuses status, out string error)
    {
        List<Discount> newDiscounts = new List<Discount>();
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

        foreach (var productId in productsByCategory)
        {
            discount.ProdcutId = productId;
            
            if (IsDiscountValid(discount, out error))
            {
                _logger.LogInformation($"{discount} is valid, let's create it");
                string discountId = Guid.NewGuid().ToString();
                discount.DiscountId = discountId;
            
                _discountsRepository.CreateDiscount(discount);
                var newDiscount = _discountsRepository.GetDiscount(discountId);

                _logger.LogInformation($"Discount created successfully, new Discount Id: {newDiscount!.DiscountId}");
                newDiscounts.Add(newDiscount);
            }
            else
            {
                _logger.LogError($"Discount {discount} is not valid. Error: {error}");
                status = Statuses.INVALID;
                return null;
            }
        }
        
        return newDiscounts;
    }

    public List<Discount> CreateDiscountsByUser(Discount discount, string userId, out Statuses status, out string error)
    {
        List<Discount> newDiscounts = new List<Discount>();
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

        foreach (var prodcutId in productsByUser)
        {
            discount.ProdcutId = prodcutId;
            
            if (IsDiscountValid(discount, out error))
            {
                _logger.LogInformation($"{discount} is valid, let's create it");
                string discountId = Guid.NewGuid().ToString();
                discount.DiscountId = discountId;
            
                _discountsRepository.CreateDiscount(discount);
                var newDiscount = _discountsRepository.GetDiscount(discountId);
            
                _logger.LogInformation($"Discount created successfully, new Discount Id: {newDiscount!.DiscountId}");
                newDiscounts.Add(newDiscount);
            }
            else
            {
                _logger.LogError($"Discount {discount} is not valid. Error: {error}");
                status = Statuses.INVALID;
                return null;
            }
        }
        
        return  newDiscounts;
    }

    public Discount UpdateDiscount(Discount discount, out Statuses status, out string error)
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

        if (IsDiscountValid(discount, out error))
        {
            _discountsRepository.UpdateDiscount(discount);
            
            var updatedDiscount = _discountsRepository.GetDiscount(discount.DiscountId);
            return updatedDiscount!;
        }
        
        status = Statuses.INVALID;
        return null;
    }

    private bool IsDiscountValid(Discount discount, out string error)
    {
        error = string.Empty;
        
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

        if (discount.ProdcutId == "" || discount.ProdcutId == null)
        {
            error = "Prodcut id can't be empty";
            return false;
        }

        if (_productsRepository.GetProduct(discount.ProdcutId) == null)
        {
            error = "Discount product must be non-null";
            return false;
        }

        return true;
    }
}