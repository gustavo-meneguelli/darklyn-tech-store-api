using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Features.Auth.DTOs;
using Application.Features.Categories.DTOs;
using Application.Features.Products.DTOs;
using Domain.Entities;

namespace IntegrationTests;

public class ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task Authenticate()
    {
        var loginDto = new LoginDto
        {
            Username = "admin",
            Password = "SenhaTeste123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/Login", loginDto);

        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse!.Token);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenExecuted()
    {
        //ARRANGE
        await Authenticate();

        //ACT
        var response = await _client.GetAsync("/api/products");

        //ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated_WhenProductIsValid()
    {
        // 1. ARRANGE
        await Authenticate();

        var newCategory = new CreateCategoryDto { Name = $"Tech-{Guid.NewGuid()}" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", newCategory);
        categoryResponse.EnsureSuccessStatusCode(); // Fail if category creation failed

        var createdCategory = await categoryResponse.Content.ReadFromJsonAsync<Category>();
        Assert.NotNull(createdCategory);
        var categoryId = createdCategory.Id;

        var newProduct = new CreateProductDto
        {
            Name = "Teclado Mecânico",
            Description = "Teclado mecânico RGB com switches Cherry MX",
            ImageUrl = "https://example.com/teclado.jpg",
            Price = 150.00m,
            CategoryId = categoryId // Use dynamic ID
        };

        // 2. ACT 
        var productHttpResponse = await _client.PostAsJsonAsync("/api/products", newProduct);

        var returnedProduct = await productHttpResponse.Content.ReadFromJsonAsync<Product>();

        // 3. ASSERT
        Assert.Equal(HttpStatusCode.Created, productHttpResponse.StatusCode);
        Assert.NotNull(returnedProduct);
        Assert.Equal("Teclado Mecânico", returnedProduct.Name);
        Assert.True(returnedProduct.Id > 0);
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenNameIsDuplicate()
    {
        // 1. ARRANGE
        await Authenticate();

        var newCategory = new CreateCategoryDto { Name = $"Tech-{Guid.NewGuid()}" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", newCategory);
        categoryResponse.EnsureSuccessStatusCode();
        var createdCategory = await categoryResponse.Content.ReadFromJsonAsync<Category>();

        // Cria o primeiro
        var product1 = new CreateProductDto 
        { 
            Name = "Duplicado", 
            Description = "Produto duplicado para teste de validação",
            ImageUrl = "https://example.com/duplicado.jpg",
            Price = 100, 
            CategoryId = createdCategory!.Id 
        };
        await _client.PostAsJsonAsync("/api/products", product1);

        // 2. ACT - Tenta criar de novo
        var response = await _client.PostAsJsonAsync("/api/products", product1);

        // 3. ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("already exists", content, StringComparison.InvariantCultureIgnoreCase);
    }
}
