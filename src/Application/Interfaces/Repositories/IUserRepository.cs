using Application.Interfaces.Generics;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByUsernameAsync(string username);
}