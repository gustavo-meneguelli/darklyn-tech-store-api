using Domain.Enums;

namespace Application.Features.ProductReviews.DTOs;

public class ProductReviewResponseDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public int UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public AvatarType UserAvatarChoice { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public bool IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
}
