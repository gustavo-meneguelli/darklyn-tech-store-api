using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Representa uma avaliação de produto feita por um usuário.
/// Diferente de Review, que é a avaliação de primeira compra (ligada a Order).
/// </summary>
public class ProductReview : Entity
{
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; } // 1-5
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;

    // Propriedades de Navegação
    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
}
