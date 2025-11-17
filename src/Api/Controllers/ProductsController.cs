using Application.DTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllProducts()
    {
        var serviceResult = service.GetAll();
        return Ok(serviceResult);
    }

    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        var serviceResult = service.GetById(id);

        if (serviceResult is null)
        {
            return NotFound();
        }
        
        return Ok(serviceResult);
    }

    [HttpPost]
    public IActionResult AddProduct(CreateProductDto dto)
    {
        var  serviceResult = service.AddProduct(dto);
        
        return CreatedAtAction(nameof(GetProductById), new { id = serviceResult.Id }, serviceResult);
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] UpdateProductDto dto)
    {
        var serviceResult = service.UpdateProduct(id, dto);

        if (serviceResult is null)
        {
            return NotFound();
        }
        
        return Ok(serviceResult);
    }
}