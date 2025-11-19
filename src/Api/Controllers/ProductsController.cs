using Application.DTO;
using Application.Enums;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var serviceResult = await service.GetAll();
        
        return Ok(serviceResult.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var serviceResult = await service.GetById(id);

        if (serviceResult.TypeResult is TypeResult.NotFound)
        {
            return NotFound(serviceResult.Message);
        }
        
        return Ok(serviceResult.Data);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(CreateProductDto dto)
    {
        var serviceResult = await service.AddProduct(dto);

        switch (serviceResult.TypeResult)
        {
            case TypeResult.Created when serviceResult.Data != null:
                return CreatedAtAction(nameof(GetProductById), new { id = serviceResult.Data.Id }, serviceResult.Data);
            case TypeResult.Duplicated:
                return Conflict(serviceResult.Message);
            default:
                return Problem("Something went wrong.");
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
    {
        var serviceResult = await service.UpdateProduct(id, dto);

        if (serviceResult.TypeResult is TypeResult.Duplicated)
        {
            return Conflict(serviceResult.Message);
        }
        
        return Ok(serviceResult.Data);
    }
}