using Domain.Entities;

namespace Application.Interfaces.Services;

public interface ITokenService
{
    public string GenerateToken(User user);
}