using System.Text.Json.Serialization;
using Domain.Common;

namespace Domain.Entities;

public class Product : Entity
{
    public string Name { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public int CategoryId { get; set; }
    
    [JsonIgnore] 
    public Category? Category { get; set; }
}