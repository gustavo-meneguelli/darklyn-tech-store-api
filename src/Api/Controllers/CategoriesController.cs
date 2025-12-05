using Application.DTO.Categories;
using Application.Enums;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await service.GetAllAsync();
        return Ok(result.Data);
    }
    
    [HttpGet("{id}"), ActionName("GetByIdAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await service.GetByIdAsync(id);

        if (result.TypeResult == TypeResult.NotFound)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCategoryDto dto)
    {
        var result = await service.AddAsync(dto);

        return result.TypeResult switch
        {
            TypeResult.Created => CreatedAtAction(nameof(GetByIdAsync), new { id = result.Data!.Id }, result.Data),
            TypeResult.Duplicated => Conflict(result.Message),
            _ => BadRequest(result.Message)
        };
    }
}