namespace Application.Features.Products.DTOs;

public class ProductResponseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    
    // Avaliações
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}
