using Domain;

namespace Repositories.Interfaces;

public interface IDiscountsRepository
{
    Task CreateDiscount(Discount discount);
    void DeleteDiscount(Discount discount);
    void UpdateDiscount(Discount discount);
    Task<Discount?> GetDiscount(string id);
    Task<Discount?> GetDiscountByProduct(string productId);
}