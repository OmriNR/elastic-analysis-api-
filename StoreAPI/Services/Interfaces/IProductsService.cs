using Domain;

namespace Services.Interfaces;

public interface IProductsService
{
    Product GetProductById(string id, out Statuses status, out string error);
    List<Product> GetProducts(List<string> ids, out Statuses status, out string error);
    List<Product> GetAllProducts(out Statuses status, out string error);
    List<string> GetAllCategories();
    Product UpdateProduct(Product product, out Statuses status, out string error);
    void DeleteProduct(string id, out Statuses status, out string error);
    Product CreateProduct(Product product, out Statuses status, out string error);
    
    List<Product> GetProductsByCategory(string category, out Statuses status, out string error);
    List<Product> GetProductsByUser(string user, out Statuses status, out string error);
}