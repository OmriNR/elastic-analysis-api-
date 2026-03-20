using Domain;

namespace Repositories.Interfaces;

public interface IDiscountsRepository
{
    Task CreateDiscount(Discount discount);
    void DeleteDiscount(Discount discount);
    void UpdateDiscount(Discount discount);
    Discount? GetDiscount(string id);
    Discount? GetDiscountByProduct(string productId);
}