using Application.Features.ProductReviews.DTOs;
using Application.Features.ProductReviews.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>
/// Gerencia avaliações de produtos.
/// </summary>
[Authorize]
[ApiController]
[Route("api/products/{productId}/reviews")]
public class ProductReviewsController(
    IProductReviewService service,
    IValidator<CreateProductReviewDto> validator)
    : MainController
{
    /// <summary>
    /// Lista todas as avaliações aprovadas de um produto.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <returns>Lista de avaliações.</returns>
    /// <response code="200">Lista de avaliações (pode estar vazia).</response>
    /// <response code="404">Produto não encontrado.</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductReviewResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByProductId(int productId)
    {
        var result = await service.GetByProductIdAsync(productId);
        return ParseResult(result);
    }

    /// <summary>
    /// Cria uma avaliação para um produto.
    /// </summary>
    /// <remarks>
    /// Cada usuário pode avaliar um produto apenas uma vez.
    /// </remarks>
    /// <param name="productId">ID do produto.</param>
    /// <param name="dto">Dados da avaliação.</param>
    /// <returns>Avaliação criada.</returns>
    /// <response code="201">Avaliação criada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="404">Produto não encontrado.</response>
    /// <response code="409">Usuário já avaliou este produto.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductReviewResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(int productId, [FromBody] CreateProductReviewDto dto)
    {
        // Garantir que o productId da rota é usado
        var dtoWithProductId = new CreateProductReviewDto
        {
            ProductId = productId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        var validationResult = await validator.ValidateAsync(dtoWithProductId);
        var errorResponse = CustomResponse(validationResult);
        if (errorResponse is not null) return errorResponse;

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.CreateAsync(userId, dtoWithProductId);

        return ParseResult(result);
    }

    /// <summary>
    /// Remove uma avaliação do usuário atual.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="reviewId">ID da avaliação.</param>
    /// <returns>NoContent se removido.</returns>
    /// <response code="204">Avaliação removida.</response>
    /// <response code="403">Não autorizado a remover esta avaliação.</response>
    /// <response code="404">Avaliação não encontrada.</response>
    [HttpDelete("{reviewId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int productId, int reviewId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await service.DeleteAsync(userId, reviewId);
        return ParseResult(result);
    }
}
