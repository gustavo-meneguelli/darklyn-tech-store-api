namespace Application.Features.ProductReviews.DTOs;

public class CreateProductReviewDto
{
    public int ProductId { get; init; }
    public int Rating { get; init; } // 1-5
    public string Comment { get; init; } = string.Empty;
}
