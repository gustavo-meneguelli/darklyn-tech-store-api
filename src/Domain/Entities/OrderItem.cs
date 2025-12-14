using Domain.Common;

namespace Domain.Entities;

public class OrderItem : Entity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; } // Preço congelado no momento da compra
    public int Quantity { get; set; }
    
    // Propriedade Calculada
    public decimal Subtotal => Quantity * UnitPrice;
    
    // Navegação
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}