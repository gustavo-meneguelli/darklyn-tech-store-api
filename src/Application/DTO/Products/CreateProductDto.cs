namespace Application.DTO.Products;

public class CreateProductDto
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int CategoryId { get; init; }
}