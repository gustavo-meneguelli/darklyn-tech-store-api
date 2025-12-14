using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.ProductReviews.Repositories;

public interface IProductReviewRepository : IRepository<ProductReview>
{
    Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId, bool onlyApproved = true);
    Task<ProductReview?> GetByProductAndUserAsync(int productId, int userId);
    Task<bool> UserHasReviewedProductAsync(int productId, int userId);
    Task<(double AverageRating, int TotalReviews)> GetProductRatingSummaryAsync(int productId);
    
    /// <summary>
    /// Busca sumário de ratings para múltiplos produtos em uma única query.
    /// Evita problema N+1 na listagem de produtos.
    /// </summary>
    Task<Dictionary<int, (double AverageRating, int TotalReviews)>> GetRatingSummaryBatchAsync(IEnumerable<int> productIds);
}


