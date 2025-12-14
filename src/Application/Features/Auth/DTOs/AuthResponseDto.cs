using Domain.Enums;

namespace Application.Features.Auth.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime IssuedAt { get; set; }
    
    // Dados de perfil
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public AvatarType AvatarChoice { get; set; }
    public bool HasCompletedFirstPurchaseReview { get; set; }
}
