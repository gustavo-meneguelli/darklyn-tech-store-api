using Application.Features.Auth.DTOs;
using Application.Features.Auth.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    IValidator<LoginDto> loginValidator,
    IValidator<UserRegisterDto> registerValidator)
    : MainController
{
    /// <summary>
    /// Realiza o login de um usuário existente.
    /// </summary>
    /// <param name="credentials">Objeto contendo nome de usuário e senha.</param>
    /// <returns>Retorna informações de autenticação incluindo token JWT, dados do usuário e expiração.</returns>
    /// <response code="200">Login realizado com sucesso. Retorna token, username, role e timestamps.</response>
    /// <response code="400">Dados inválidos (ex: campos vazios).</response>
    /// <response code="401">Usuário ou senha inválidos.</response>
    [HttpPost("Login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync(LoginDto credentials)
    {
        var validationResult = await loginValidator.ValidateAsync(credentials);

        var errorResponse = CustomResponse(validationResult);
        if (errorResponse is not null) return errorResponse;

        var result = await authService.LoginAsync(credentials);

        return ParseResult(result);
    }

    /// <summary>
    /// Registra um novo usuário comum no sistema.
    /// </summary>
    /// <remarks>
    /// O usuário criado terá automaticamente o perfil de **Common**.
    /// Após o registro, retorna as informações de autenticação para login automático.
    /// </remarks>
    /// <param name="request">Dados para registro (Nome de usuário e Senha).</param>
    /// <returns>Informações de autenticação incluindo token JWT.</returns>
    /// <response code="200">Usuário registrado com sucesso. Retorna dados de autenticação.</response>
    /// <response code="400">Dados inválidos (ex: senha vazia).</response>
    /// <response code="409">Já existe um usuário com este nome.</response>
    [HttpPost("Register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync(UserRegisterDto request)
    {
        var validationResult = await registerValidator.ValidateAsync(request);

        var errorResponse = CustomResponse(validationResult);
        if (errorResponse is not null) return errorResponse;

        var result = await authService.RegisterAsync(request);

        return ParseResult(result);
    }
}
