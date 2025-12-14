using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove everything related to DbContext to avoid conflict with Npgsql registered in Program.cs
            
            // 1. Generic Options
            var dbContextOptions = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbContextOptions != null) services.Remove(dbContextOptions);

            // 2. Non-Generic Options (Critical for EF Core to not see multiple providers)
            var dbContextOptionsNonGeneric = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions));
            if (dbContextOptionsNonGeneric != null) services.Remove(dbContextOptionsNonGeneric);

            // 3. The Context Itself
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(AppDbContext));
            if (dbContext != null) services.Remove(dbContext);

            // 4. Options Configuration
            var dbContextConfig = services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>));
            if (dbContextConfig != null) services.Remove(dbContextConfig);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");

                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();
        });

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "AdminSettings:Password", "SenhaTeste123" },
                { "ConnectionStrings:DefaultConnection", "Host=localhost;Database=TestDb;Username=postgres;Password=postgres" },
                { "JwtSettings:SecretKey", "SuperSecretKeyForTestingPurposesOnly123!" } 
            }!);
        });
    }
}