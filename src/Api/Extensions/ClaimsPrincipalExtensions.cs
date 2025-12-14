using System.Security.Claims;
using Domain.Constants;

namespace Api.Extensions;

/// <summary>
/// Extensões para ClaimsPrincipal que facilitam a extração de informações do usuário autenticado.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Obtém o ID do usuário autenticado a partir dos claims JWT.
    /// </summary>
    /// <param name="user">ClaimsPrincipal do usuário autenticado.</param>
    /// <returns>ID do usuário como inteiro.</returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Lançada quando o claim NameIdentifier não está presente ou é inválido.
    /// </exception>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out var userId))
            throw new UnauthorizedAccessException(ErrorMessages.UserIdNotFoundInClaims);
        
        return userId;
    }
}

