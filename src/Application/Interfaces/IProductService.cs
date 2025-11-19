using Application.DTO;
using Application.Utilities;
using Domain.Models;

namespace Application.Interfaces;

public interface IProductService
{
    Task<Result<IEnumerable<Product>>> GetAll();
    Task<Result<Product?>> GetById(int id);
    Task<Result<Product>> AddProduct(CreateProductDto dto);
    Task<Result<Product?>> UpdateProduct(int id, UpdateProductDto dto);
}