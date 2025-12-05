using Application.Interfaces.Generics;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<bool> ExistsByNameAsync(string name);
}