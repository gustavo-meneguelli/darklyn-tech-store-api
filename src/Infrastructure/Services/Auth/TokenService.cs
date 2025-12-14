using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Application.Features.Auth.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Auth;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public AuthResponseDto GenerateAuthResponse(User user)
    {
        // Validações de configuração obrigatórias
        var secretKey = configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey não está configurado");

        var expirationHours = int.TryParse(
            configuration["JwtSettings:ExpirationHours"],
            out var hours) ? hours : 8;

        // Configurações opcionais (podem ser null)
        var issuer = configuration["JwtSettings:Issuer"];
        var audience = configuration["JwtSettings:Audience"];

        // Timestamps
        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddHours(expirationHours);

        // Claims
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // Security
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Token
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: issuedAt,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            Token = tokenString,
            Username = user.Username,
            Role = user.Role.ToString(),
            ExpiresAt = expiresAt,
            IssuedAt = issuedAt,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Initials = user.Initials,
            AvatarChoice = user.AvatarChoice,
            HasCompletedFirstPurchaseReview = user.HasCompletedFirstPurchaseReview
        };
    }
}
