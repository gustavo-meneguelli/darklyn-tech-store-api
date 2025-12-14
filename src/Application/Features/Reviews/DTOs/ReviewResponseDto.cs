using Domain.Enums;

namespace Application.Features.Reviews.DTOs;

public class ReviewResponseDto
{
    public int Id { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string UserInitials { get; init; } = string.Empty;
    public AvatarType UserAvatarChoice { get; init; }
    public bool IsApproved { get; init; }
}
