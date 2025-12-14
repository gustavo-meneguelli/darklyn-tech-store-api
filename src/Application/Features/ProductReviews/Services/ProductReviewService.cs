using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.ProductReviews.DTOs;
using Application.Features.ProductReviews.Repositories;
using Application.Features.Products.Repositories;
using Application.Features.Orders.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;

namespace Application.Features.ProductReviews.Services;

public class ProductReviewService(
    IProductReviewRepository productReviewRepository,
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    IProfanityFilterService profanityFilter,
    IMapper mapper,
    IUnitOfWork unitOfWork) : IProductReviewService
{
    public async Task<Result<IEnumerable<ProductReviewResponseDto>>> GetByProductIdAsync(int productId)
    {
        var product = await productRepository.GetByIdAsync(productId);
        if (product is null)
        {
            return Result<IEnumerable<ProductReviewResponseDto>>.NotFound(ErrorMessages.ProductNotFound);
        }

        var reviews = await productReviewRepository.GetByProductIdAsync(productId, onlyApproved: true);

        var response = mapper.Map<IEnumerable<ProductReviewResponseDto>>(reviews);

        return Result<IEnumerable<ProductReviewResponseDto>>.Success(response);
    }

    public async Task<Result<ProductReviewResponseDto>> CreateAsync(int userId, CreateProductReviewDto dto)
    {
        var product = await productRepository.GetByIdAsync(dto.ProductId);
        if (product is null)
        {
            return Result<ProductReviewResponseDto>.NotFound(ErrorMessages.ProductNotFound);
        }

        // Validar se usuário já avaliou este produto
        var alreadyReviewed = await productReviewRepository.UserHasReviewedProductAsync(dto.ProductId, userId);
        if (alreadyReviewed)
        {
            return Result<ProductReviewResponseDto>.Duplicate(ErrorMessages.AlreadyReviewedProduct);
        }

        // Validar se usuário comprou o produto (pedido pago ou entregue)
        var hasPurchased = await orderRepository.UserHasPurchasedProductAsync(userId, dto.ProductId);
        if (!hasPurchased)
        {
            return Result<ProductReviewResponseDto>.Failure(ErrorMessages.MustPurchaseToReview);
        }

        // Rejeitar se contiver palavras proibidas
        if (profanityFilter.ContainsProfanity(dto.Comment))
        {
            return Result<ProductReviewResponseDto>.Failure(ErrorMessages.ProfanityDetected);
        }

        var review = mapper.Map<ProductReview>(dto);
        review.UserId = userId;
        review.IsApproved = true;

        await productReviewRepository.AddAsync(review);
        await unitOfWork.CommitAsync();

        // Buscar novamente para ter dados do usuário
        var savedReview = await productReviewRepository.GetByIdAsync(review.Id);

        var response = mapper.Map<ProductReviewResponseDto>(savedReview);

        return Result<ProductReviewResponseDto>.Created(response);
    }

    public async Task<Result<ProductReviewResponseDto?>> DeleteAsync(int userId, int reviewId)
    {
        var review = await productReviewRepository.GetByIdAsync(reviewId);
        if (review is null)
        {
            return Result<ProductReviewResponseDto?>.NotFound(ErrorMessages.ReviewNotFound);
        }

        // Usuário só pode deletar própria avaliação
        if (review.UserId != userId)
        {
            return Result<ProductReviewResponseDto?>.Unauthorized(ErrorMessages.AccessDenied);
        }

        await productReviewRepository.DeleteAsync(review);
        await unitOfWork.CommitAsync();

        return Result<ProductReviewResponseDto?>.NoContent();
    }
}

