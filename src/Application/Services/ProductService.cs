using Application.DTO;
using Application.Interfaces;
using Domain.Models;

namespace Application.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public IEnumerable<Product> GetAll()
    {
        return repository.GetProducts();
    }

    public Product? GetById(int id)
    {
        return repository.GetProduct(id);
    }

    public Product AddProduct(CreateProductDto dto)
    {
        var product = new Product {  Name = dto.Name, Price = dto.Price };
        repository.AddProduct(product);
        return product;
    }

    public Product? UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = repository.GetProduct(id);

        if (product is null)
        {
            return null;
        }

        if (dto.Name != product.Name && dto.Name != string.Empty)
        {
            product.Name = dto.Name;
        }

        if (dto.Price != product.Price && dto.Price != 0)
        {
            product.Price = dto.Price;
        }

        repository.UpdateProduct(product);
        return product;
    }
}