using Application.Common.Models;
using Application.DTO.Categories;

namespace Application.Interfaces.Services;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryResponseDto>>> GetAllAsync();
    
    Task<Result<CategoryResponseDto>> GetByIdAsync(int id);
    
    Task<Result<CategoryResponseDto>> AddAsync(CreateCategoryDto dto);
}