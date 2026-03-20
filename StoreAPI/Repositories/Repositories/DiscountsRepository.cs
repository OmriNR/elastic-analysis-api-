using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Repositories;

public class DiscountsRepository : IDiscountsRepository
{
    private readonly AppDBContext _context;

    public DiscountsRepository(AppDBContext context)
    {
        _context = context;
    }


    public async Task CreateDiscount(Discount discount)
    {
        _context.Add(discount);
        await _context.SaveChangesAsync();
    }

    public void DeleteDiscount(Discount discount)
    {
        _context.Discounts.Remove(discount);
        _context.SaveChanges();
    }

    public void UpdateDiscount(Discount discount)
    {
        var local = _context.Discounts.Local.FirstOrDefault(d => d.DiscountId == discount.DiscountId);

        if (local != null)
        {
            _context.Entry(local).CurrentValues.SetValues(discount);
        }
        
        _context.Discounts.Update(discount);
        _context.SaveChanges();
    }

    public Discount? GetDiscount(string id)
    {
        var discount = _context.Discounts.Find(id);
        return discount;
    }

    public Discount? GetDiscountByProduct(string productId)
    {
        var discount = _context.Discounts
            .Where(d => d.Products.Any(p => p == productId))
            .Where(d => d.ExpiredAt >=  DateTime.Now)
            .OrderByDescending(d => d.ExpiredAt)
            .FirstOrDefault();
        
        return discount;
    }
}