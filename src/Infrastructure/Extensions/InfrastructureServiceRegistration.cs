using Application.Common.Interfaces;
using Application.Features.Auth.Repositories;
using Application.Features.Carts.Repositories;
using Application.Features.Categories.Repositories;
using Application.Features.Orders.Repositories;
using Application.Features.ProductReviews.Repositories;
using Application.Features.Products.Repositories;
using Application.Features.Reviews.Repositories;
using Infrastructure.Data;
using Infrastructure.Generics;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                             ?? configuration["DATABASE_URL"];

        // Em ambiente de teste, a connection string pode não estar presente imediatamente ou será substituída.
        // Não lançamos exceção aqui para permitir que o WebApplicationFactory funcione (ele substitui o DbContext depois).
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=localhost;Database=DarklynStore;Username=postgres;Password=postgres";
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IProductReviewRepository, ProductReviewRepository>();

        // Infrastructure Services
        services.AddScoped<IProfanityFilterService, ProfanityFilterService>();
        services.AddScoped<IPasswordHash, PasswordHash>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<DbSeeder>(); // Seeder

        // JWT Authentication Config
        AddJwtAuthentication(services, configuration);

        return services;
    }

    private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IConfiguration>((options, config) =>
            {
                var secretKey = config["JwtSettings:SecretKey"]
                                ?? throw new InvalidOperationException("JwtSettings:SecretKey is null");

                var issuer = config["JwtSettings:Issuer"];
                var audience = config["JwtSettings:Audience"];

                var keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
                var key = new SymmetricSecurityKey(keyBytes);

                // TODO: Em produção, deve ser true
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ValidateIssuer = !string.IsNullOrEmpty(issuer),
                    ValidIssuer = issuer,
                    ValidateAudience = !string.IsNullOrEmpty(audience),
                    ValidAudience = audience
                };
            });

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();
    }
}
