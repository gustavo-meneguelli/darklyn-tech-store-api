using Application.Features.Reviews.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Generics;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReviewRepository(AppDbContext context) : Repository<Review>(context), IReviewRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Review?> GetByIdWithUserAsync(int id)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Review?> GetByUserIdAsync(int userId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == userId);
    }

    public async Task<IEnumerable<Review>> GetPublicReviewsAsync(int limit)
    {
        return await _context.Reviews
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .Include(r => r.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetAllWithFiltersAsync(int? rating = null, bool? approved = null)
    {
        var query = _context.Reviews
            .Include(r => r.User)
            .AsQueryable();

        if (rating.HasValue)
            query = query.Where(r => r.Rating == rating.Value);

        if (approved.HasValue)
            query = query.Where(r => r.IsApproved == approved.Value);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UserHasReviewedAsync(int userId)
    {
        return await _context.Reviews.AnyAsync(r => r.UserId == userId);
    }
}
