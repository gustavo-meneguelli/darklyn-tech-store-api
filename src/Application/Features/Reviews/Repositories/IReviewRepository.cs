using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.Reviews.Repositories;

public interface IReviewRepository : IRepository<Review>
{
    Task<Review?> GetByIdWithUserAsync(int id);
    Task<Review?> GetByUserIdAsync(int userId);
    Task<IEnumerable<Review>> GetPublicReviewsAsync(int limit);
    Task<IEnumerable<Review>> GetAllWithFiltersAsync(int? rating = null, bool? approved = null);
    Task<bool> UserHasReviewedAsync(int userId);
}
