using Domain.Models;

namespace Application.Interfaces;

public interface IProductService
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
}