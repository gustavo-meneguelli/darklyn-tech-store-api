using Application.Common.Models;
using Application.Features.Reviews.DTOs;

namespace Application.Features.Reviews.Services;

public interface IReviewService
{
    Task<Result<ReviewResponseDto>> CreateAsync(int userId, CreateReviewDto dto);
    Task<Result<IEnumerable<ReviewResponseDto>>> GetPublicAsync(int limit);
    Task<Result<bool>> HasReviewedAsync(int userId);
    Task<Result<IEnumerable<ReviewResponseDto>>> GetAllAsync(int? rating, bool? approved);
    Task<Result<ReviewResponseDto>> SetApprovalAsync(int reviewId, bool approved);
}
