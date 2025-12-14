using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Api.Extensions;

public static class AppServiceExtension
{
    // Método AddApplicationServices removido para evitar recursão e ambiguidade.
    // A chamada agora é feita diretamente no Program.cs para as camadas Application e Infrastructure.

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT desta maneira: Bearer {seu token}"
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }



    public static async Task UseDbSeeder(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;

        try
        {
            var dbContext = services.GetRequiredService<AppDbContext>();

            // Aplica migrations automaticamente no startup
            if (dbContext.Database.IsRelational())
            {
                await dbContext.Database.MigrateAsync();
            }

            // Seed de produção (sempre roda)
            var seeder = services.GetRequiredService<DbSeeder>();
            await seeder.SeedAsync();

        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }

    public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration config)
    {
        var allowedOrigin = config["CorsSettings:AllowedOrigins"]
                            ?? throw new InvalidOperationException("CorsSettings:AllowedOrigins is missing in appsettings.");

        services.AddCors(options =>
        {
            options.AddPolicy("WebUiPolicy", builder =>
            {
                builder
                    .WithOrigins(allowedOrigin)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
}