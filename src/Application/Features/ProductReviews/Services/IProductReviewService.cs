using Application.Common.Models;
using Application.Features.ProductReviews.DTOs;

namespace Application.Features.ProductReviews.Services;

public interface IProductReviewService
{
    Task<Result<IEnumerable<ProductReviewResponseDto>>> GetByProductIdAsync(int productId);
    Task<Result<ProductReviewResponseDto>> CreateAsync(int userId, CreateProductReviewDto dto);
    Task<Result<ProductReviewResponseDto?>> DeleteAsync(int userId, int reviewId);
}
