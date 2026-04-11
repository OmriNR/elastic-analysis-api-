using Domain;
namespace Repositories.Interfaces;

public interface IDiscountsRepository
{
    void CreateDiscount(Discount discount);
    void DeleteDiscount(Discount discount);
    void UpdateDiscount(Discount discount);
    Discount? GetDiscount(string id);
    Discount? GetDiscountByProduct(string productId);
}