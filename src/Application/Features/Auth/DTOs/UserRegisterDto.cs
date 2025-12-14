using Domain.Enums;

namespace Application.Features.Auth.DTOs;

public class UserRegisterDto
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public AvatarType AvatarChoice { get; init; } = AvatarType.Initials;
}
