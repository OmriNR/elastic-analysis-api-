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
    }

    public void UpdateDiscount(Discount discount)
    {
        _context.Discounts.Update(discount);
    }

    public async Task<Discount?> GetDiscount(string id)
    {
        var discount = await _context.Discounts.FindAsync(id);

        return discount;
    }

    public async Task<Discount?> GetDiscountByProduct(string productId)
    {
        var discount = await _context.Discounts
            .Where(d => d.Products.Any(p => p == productId))
            .Where(d => d.ExpiredAt >=  DateTime.Now)
            .OrderByDescending(d => d.ExpiredAt)
            .FirstOrDefaultAsync();
        
        return discount;
    }
}