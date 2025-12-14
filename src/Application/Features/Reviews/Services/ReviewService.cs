using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Auth.Repositories;
using Application.Features.Orders.Repositories;
using Application.Features.Reviews.DTOs;
using Application.Features.Reviews.Repositories;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Reviews.Services;


public class ReviewService(
    IReviewRepository reviewRepository,
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IProfanityFilterService profanityFilter,
    IUnitOfWork unitOfWork) : IReviewService
{
    public async Task<Result<ReviewResponseDto>> CreateAsync(int userId, CreateReviewDto dto)
    {
        if (await reviewRepository.UserHasReviewedAsync(userId))
            return Result<ReviewResponseDto>.Duplicate(ErrorMessages.AlreadyReviewed);

        var order = await orderRepository.GetByIdWithItemsAsync(dto.OrderId);

        if (order is null)
            return Result<ReviewResponseDto>.NotFound(ErrorMessages.OrderNotFound);

        if (order.UserId != userId)
            return Result<ReviewResponseDto>.Unauthorized(ErrorMessages.NoPermissionToReview);

        if (order.Status != OrderStatus.Pending)
            return Result<ReviewResponseDto>.NotFound(ErrorMessages.OrderAlreadyProcessed);

        var review = new Review
        {
            UserId = userId,
            OrderId = dto.OrderId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            IsApproved = !profanityFilter.ContainsProfanity(dto.Comment)
        };

        await reviewRepository.AddAsync(review);

        order.Status = OrderStatus.Paid;

        var user = await userRepository.GetByIdAsync(userId);
        if (user is not null)
        {
            user.HasCompletedFirstPurchaseReview = true;
        }

        await unitOfWork.CommitAsync();

        var savedReview = await reviewRepository.GetByIdWithUserAsync(review.Id);
        var response = MapToDto(savedReview!);

        return Result<ReviewResponseDto>.Success(response);
    }

    public async Task<Result<IEnumerable<ReviewResponseDto>>> GetPublicAsync(int limit)
    {
        var reviews = await reviewRepository.GetPublicReviewsAsync(limit);
        var response = reviews.Select(MapToDto);
        return Result<IEnumerable<ReviewResponseDto>>.Success(response);
    }

    public async Task<Result<bool>> HasReviewedAsync(int userId)
    {
        var hasReviewed = await reviewRepository.UserHasReviewedAsync(userId);
        return Result<bool>.Success(hasReviewed);
    }

    public async Task<Result<IEnumerable<ReviewResponseDto>>> GetAllAsync(int? rating, bool? approved)
    {
        var reviews = await reviewRepository.GetAllWithFiltersAsync(rating, approved);
        var response = reviews.Select(MapToDto);
        return Result<IEnumerable<ReviewResponseDto>>.Success(response);
    }

    public async Task<Result<ReviewResponseDto>> SetApprovalAsync(int reviewId, bool approved)
    {
        var review = await reviewRepository.GetByIdWithUserAsync(reviewId);

        if (review is null)
            return Result<ReviewResponseDto>.NotFound(ErrorMessages.ReviewNotFound);

        review.IsApproved = approved;
        await unitOfWork.CommitAsync();

        var response = MapToDto(review);
        return Result<ReviewResponseDto>.Success(response);
    }

    private static ReviewResponseDto MapToDto(Review review)
    {
        return new ReviewResponseDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UserName = review.User.FullName,
            UserInitials = review.User.Initials,
            UserAvatarChoice = review.User.AvatarChoice,
            IsApproved = review.IsApproved
        };
    }
}

