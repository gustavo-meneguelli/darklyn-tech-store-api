using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Representa uma avaliação de cliente após a primeira compra.
/// </summary>
public class Review : Entity
{
    public int UserId { get; set; }
    public int OrderId { get; set; }
    public int Rating { get; set; } // 1-5
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;

    // Propriedades de Navegação
    public User User { get; set; } = null!;
    public Order Order { get; set; } = null!;
}
