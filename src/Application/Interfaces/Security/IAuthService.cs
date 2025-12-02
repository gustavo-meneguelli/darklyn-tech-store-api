using Application.Common.Models;
using Application.DTO.Auth;

namespace Application.Interfaces.Security;

public interface IAuthService
{
    public Task<Result<string>> LoginAsync(LoginDto login);
    public Task<Result<string>> RegisterAsync(UserRegisterDto dto);
}