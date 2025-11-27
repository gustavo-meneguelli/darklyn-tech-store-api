using Application.DTO;
using Application.Utilities;
using Domain.Models;

namespace Application.Interfaces;

public interface IProductService
{
    Task<Result<IEnumerable<Product>>> GetAllAsync();
    Task<Result<Product?>> GetByIdAsync(int id);
    Task<Result<Product>> AddAsync(CreateProductDto dto);
    Task<Result<Product?>> UpdateAsync(int id, UpdateProductDto dto);
    Task<Result<Product?>> DeleteAsync(int id);
}