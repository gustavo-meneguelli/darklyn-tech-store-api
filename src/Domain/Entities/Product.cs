
using Domain.Common;

namespace Domain.Entities;

public class Product : Entity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();

    public override string ToString()
    {
        return $"Product {{ Id = {Id}, Name = \"{Name}\", Price = {Price:C}, CategoryId = {CategoryId} }}";
    }
}
