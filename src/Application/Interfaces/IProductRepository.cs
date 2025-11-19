using Domain.Models;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProducts();
    Task<Product?> GetProduct(int id);
    Task<Product?> GetProductByName(string name);
    Task<Product> AddProduct(Product product);
    Task<Product> UpdateProduct(Product product);
}