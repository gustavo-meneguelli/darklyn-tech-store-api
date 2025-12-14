namespace Application.Features.Reviews.DTOs;

public class CreateReviewDto
{
    public int OrderId { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
}
