using Domain.Common;

namespace Domain.Entities;

public class CartItem : Entity
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Preço congelado no momento da adição
    
    // Propriedade Calculada
    public decimal Subtotal => Quantity * UnitPrice;
    
    // Calcula economia unitária com base no preço atual do produto
    // Se produto não estiver carregado (null), considera economia 0
    public decimal UnitSavings => Product != null ? Product.Price - UnitPrice : 0;

    // Calcula economia total desta linha
    public decimal TotalSavings => UnitSavings * Quantity;
    
    // Navegação
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
