using Domain;

namespace Services.Interfaces;

public interface IDiscountsService
{
    Discount GetDiscountById(string id, out Statuses status, out string error);
    
    Discount GetDiscountByProduct(string productId, out Statuses status, out string error);
    
    Discount CreateDiscount(Discount discount, out Statuses status, out string error);
    
    Discount CreateDiscountByCategory(Discount discount, string category,  out Statuses status, out string error);
    
    Discount CreateDiscountByUser(Discount discount, string userId,  out Statuses status, out string error);
    
    //Discount UpdateDiscount(Discount discount,  out Statuses status, out string error);
}