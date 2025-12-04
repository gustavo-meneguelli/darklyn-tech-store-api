using Application.Common.Models;
using Application.DTO.Products;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IProductService
{
    Task<Result<PagedResult<ProductResponseDto>>> GetAllAsync(PaginationParams paginationParams);
    Task<Result<ProductResponseDto?>> GetByIdAsync(int id);
    Task<Result<ProductResponseDto>> AddAsync(CreateProductDto dto);
    Task<Result<ProductResponseDto?>> UpdateAsync(int id, UpdateProductDto dto);
    Task<Result<ProductResponseDto?>> DeleteAsync(int id);
}