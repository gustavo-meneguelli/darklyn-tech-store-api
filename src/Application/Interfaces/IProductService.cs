using Application.DTO;
using Domain.Models;

namespace Application.Interfaces;

public interface IProductService
{
    IEnumerable<Product> GetAll();
    Product? GetById(int id);
    Product AddProduct(CreateProductDto dto);
    Product? UpdateProduct(int id, UpdateProductDto dto);
}