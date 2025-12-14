using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class User : Entity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Common;
    
    // Campos de perfil
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public AvatarType AvatarChoice { get; set; } = AvatarType.Initials;
    public bool HasCompletedFirstPurchaseReview { get; set; } = false;
    
    // Propriedades calculadas (não mapeadas para o banco)
    public string FullName => string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
        ? Username
        : $"{FirstName} {LastName}".Trim();
    
    public string Initials => GetInitials();

    public override string ToString()
    {
        return $"User {{ Id = {Id}, Username = \"{Username}\", Role = {Role} }}";
    }
    
    private string GetInitials()
    {
        if (!string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName))
        {
            var first = FirstName.Length > 0 ? FirstName[0].ToString().ToUpper() : "";
            var last = LastName.Length > 0 ? LastName[0].ToString().ToUpper() : "";
            return $"{first}{last}";
        }
        
        // Fallback para username
        return Username.Length > 0 ? Username[0].ToString().ToUpper() : "U";
    }
}
