using Application.DTO;
using Application.Interfaces;
using Application.Utilities;
using Domain.Models;

namespace Application.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public async Task<Result<IEnumerable<Product>>> GetAll()
    {
        var products = await repository.GetProducts();
        
        return Result<IEnumerable<Product>>.Success(products);
    }

    public async Task<Result<Product?>> GetById(int id)
    {
        var product = await repository.GetProduct(id);

        return product is null
            ? Result<Product?>.NotFound("No product were found with this ID.")
            : Result<Product?>.Success(product);
    }

    public async Task<Result<Product>> AddProduct(CreateProductDto dto)
    {
        bool productExists = await repository.GetProductByName(dto.Name) is not null;

        if (productExists)
        {
            return Result<Product>.Duplicate("Product with that name already exists.");
        }
        
        var product = new Product { Name = dto.Name, Price = dto.Price };
        await repository.AddProduct(product);
        return Result<Product>.Created(product);
    }

    public async Task<Result<Product?>> UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = await repository.GetProduct(id);

        if (product is null)
        {
            return Result<Product?>.NotFound("No products were found with this ID.");
        }

        var productNameAlreadyExists = await repository.GetProductByName(dto.Name) is not null;

        if (productNameAlreadyExists && dto.Name != product.Name) 
        {
            return Result<Product?>.Duplicate("Product with that name already exists.");
        }
        
        if (dto.Name != product.Name && dto.Name != string.Empty)
        {
            product.Name = dto.Name;
        }
        
        if (dto.Price != product.Price && dto.Price != 0)
        {
            product.Price = dto.Price;
        }

        await repository.UpdateProduct(product);
        return Result<Product?>.Success(product);
    }
}