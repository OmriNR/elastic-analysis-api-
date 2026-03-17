using Domain;

namespace Services.Interfaces;

public interface IDiscountService
{
    Discount GetById(string id, out Statuses status, out string error);
    Discount GetByDiscount(string productId, out Statuses status, out string error);
    Discount CreateDiscount(Discount product, out Statuses status, out string error);
    Discount UpdateDiscount(Discount product, out Statuses status, out string error);
    void DeleteDiscount(string id, out Statuses status, out string error);
}