using Domain.Common;

namespace Domain.Entities;

public class Cart : Entity
{
    public int UserId { get; set; }
    
    // Navegação
    public User User { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    
    // Propriedade Calculada
    public decimal TotalAmount => Items.Sum(item => item.Subtotal);
    public decimal TotalSavings => Items.Sum(item => item.TotalSavings);
}
