using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly AppDBContext _context;

    public ProductsRepository(AppDBContext context)
    {
        _context = context;
    }
    
    public void UpdateProduct(Product product)
    {
        var local = _context.Products.Local.FirstOrDefault(p => p.ProductId == product.ProductId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }
        
        _context.Products.Update(product);
        _context.SaveChanges();
    }

    public void CreateProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void DeleteProduct(Product product)
    {
        _context.Products.Remove(product);
        _context.SaveChanges();
    }

    public Product? GetProduct(string id)
    {
        var product = _context.Products.Find(id);
        return product;
    }

    public List<Product> GetProducts(List<string> ids)
    {
        var prodcuts = _context.Products.Where(p => ids.Contains(p.ProductId.ToString())).ToList<Product>();

        return prodcuts;
    }

    public List<Product> GetProductsByCategory(string category)
    {
        var products =  _context.Products.Where(p => p.Category == category).ToList<Product>();

        return products;
    }

    public List<Product> GetProductsByUser(string user)
    {
        var porducts = _context.Products.Where(p => p.OwnerId ==  user).ToList<Product>();
        return porducts;
    }
}