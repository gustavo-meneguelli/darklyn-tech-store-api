using Application.Features.ProductReviews.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Generics;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductReviewRepository(AppDbContext context) : Repository<ProductReview>(context), IProductReviewRepository
{
    private readonly AppDbContext _context = context;

    public override async Task<ProductReview?> GetByIdAsync(int id)
    {
        return await _context.ProductReviews
            .Include(pr => pr.User)
            .FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId, bool onlyApproved = true)
    {
        var query = _context.ProductReviews
            .Include(pr => pr.User)
            .Where(pr => pr.ProductId == productId);

        if (onlyApproved)
        {
            query = query.Where(pr => pr.IsApproved);
        }

        return await query
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
    }

    public async Task<ProductReview?> GetByProductAndUserAsync(int productId, int userId)
    {
        return await _context.ProductReviews
            .FirstOrDefaultAsync(pr => pr.ProductId == productId && pr.UserId == userId);
    }

    public async Task<bool> UserHasReviewedProductAsync(int productId, int userId)
    {
        return await _context.ProductReviews
            .AnyAsync(pr => pr.ProductId == productId && pr.UserId == userId);
    }

    public async Task<(double AverageRating, int TotalReviews)> GetProductRatingSummaryAsync(int productId)
    {
        var approvedReviews = await _context.ProductReviews
            .Where(pr => pr.ProductId == productId && pr.IsApproved)
            .ToListAsync();

        if (approvedReviews.Count == 0)
        {
            return (0, 0);
        }

        var average = approvedReviews.Average(pr => pr.Rating);
        return (Math.Round(average, 1), approvedReviews.Count);
    }

    /// <summary>
    /// Busca sumário de ratings para múltiplos produtos em uma única query.
    /// Utiliza GROUP BY no banco para calcular média e contagem de forma eficiente.
    /// </summary>
    public async Task<Dictionary<int, (double AverageRating, int TotalReviews)>> GetRatingSummaryBatchAsync(IEnumerable<int> productIds)
    {
        var productIdsList = productIds.ToList();
        
        if (productIdsList.Count == 0)
            return new Dictionary<int, (double AverageRating, int TotalReviews)>();

        // Query única com GROUP BY - evita N+1
        var summaries = await _context.ProductReviews
            .Where(pr => productIdsList.Contains(pr.ProductId) && pr.IsApproved)
            .GroupBy(pr => pr.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                AverageRating = Math.Round(g.Average(pr => pr.Rating), 1),
                TotalReviews = g.Count()
            })
            .ToDictionaryAsync(
                x => x.ProductId,
                x => (x.AverageRating, x.TotalReviews)
            );

        return summaries;
    }
}

