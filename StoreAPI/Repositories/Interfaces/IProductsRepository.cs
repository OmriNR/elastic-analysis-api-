using Domain;

namespace Repositories.Interfaces;

public interface IProductsRepository
{
    void UpdateProduct(Product product);
    void CreateProduct(Product product);
    void DeleteProduct(Product product);
    Product? GetProduct(string id);
    List<Product> GetProducts(List<string> ids);
}