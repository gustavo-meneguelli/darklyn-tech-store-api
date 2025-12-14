using Application.Features.Reviews.DTOs;
using Application.Features.Reviews.Services;
using Api.Extensions;
using FluentValidation;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(
    IReviewService reviewService,
    IValidator<CreateReviewDto> createReviewValidator) : MainController
{
    /// <summary>
    /// Cria uma avaliação de primeira compra.
    /// </summary>
    /// <remarks>
    /// Também marca o pedido como pago e o usuário como tendo completado sua primeira avaliação.
    /// </remarks>
    /// <param name="dto">Dados da avaliação</param>
    /// <returns>Avaliação criada (aguardando aprovação)</returns>
    /// <response code="200">Avaliação criada com sucesso</response>
    /// <response code="400">Dados inválidos ou usuário já avaliou</response>
    /// <response code="404">Pedido não encontrado</response>
    /// <response code="401">Não autorizado</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateReviewDto dto)
    {
        var validationResult = await createReviewValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

        var userId = User.GetUserId();
        var result = await reviewService.CreateAsync(userId, dto);
        return ParseResult(result);
    }

    /// <summary>
    /// Lista avaliações públicas aprovadas.
    /// </summary>
    /// <param name="limit">Número máximo de avaliações (padrão: 10)</param>
    /// <returns>Lista de avaliações aprovadas</returns>
    /// <response code="200">Avaliações encontradas</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicAsync([FromQuery] int limit = 10)
    {
        var result = await reviewService.GetPublicAsync(limit);
        return ParseResult(result);
    }

    /// <summary>
    /// Verifica se o usuário já realizou sua avaliação de primeira compra.
    /// </summary>
    /// <returns>Booleano indicando se já avaliou</returns>
    /// <response code="200">Retorna true se já avaliou, false caso contrário</response>
    [Authorize]
    [HttpGet("has-reviewed")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> HasReviewedAsync()
    {
        var userId = User.GetUserId();
        var result = await reviewService.HasReviewedAsync(userId);
        return ParseResult(result);
    }

    /// <summary>
    /// [Admin] Lista todas as avaliações com filtros opcionais.
    /// </summary>
    /// <param name="rating">Filtro por nota (1-5)</param>
    /// <param name="approved">Filtro por status de aprovação</param>
    /// <returns>Lista de todas as avaliações</returns>
    /// <response code="200">Avaliações encontradas</response>
    /// <response code="403">Apenas administradores podem acessar</response>
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("admin")]
    [ProducesResponseType(typeof(IEnumerable<ReviewResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllAsync([FromQuery] int? rating = null, [FromQuery] bool? approved = null)
    {
        var result = await reviewService.GetAllAsync(rating, approved);
        return ParseResult(result);
    }

    /// <summary>
    /// [Admin] Aprova ou reprova uma avaliação.
    /// </summary>
    /// <param name="id">ID da avaliação</param>
    /// <param name="dto">Dados de aprovação</param>
    /// <returns>Avaliação atualizada</returns>
    /// <response code="200">Avaliação atualizada com sucesso</response>
    /// <response code="404">Avaliação não encontrada</response>
    /// <response code="403">Apenas administradores podem acessar</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/approve")]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetApprovalAsync(int id, [FromBody] SetApprovalDto dto)
    {
        var result = await reviewService.SetApprovalAsync(id, dto.Approved);
        return ParseResult(result);
    }
}
