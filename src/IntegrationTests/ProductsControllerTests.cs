using System.Net;
using System.Net.Http.Json;
using Application.DTO;

namespace IntegrationTests;

public class ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenExecuted()
    {
        //ARRANGE
        var response = await _client.GetAsync("/api/products");

        //ACT
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro na API: {errorContent}");
        }

        //ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Post_ShouldReturnCreated_WhenProductIsValid()
    {
        // 1. ARRANGE
        var newProduct = new CreateProductDto
        {
            Name = "Teclado Mecânico",
            Price = 150.00m
        };

        // 2. ACT 
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // 3. ASSERT
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var returnedProduct = await response.Content.ReadFromJsonAsync<Domain.Models.Product>();
        
        Assert.NotNull(returnedProduct);
        Assert.Equal("Teclado Mecânico", returnedProduct.Name);
        Assert.True(returnedProduct.Id > 0); // O banco deve ter gerado um ID
    }
}