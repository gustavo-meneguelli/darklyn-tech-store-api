using Application.Common.Models;
using Application.DTO.Products;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public class ProductService(IProductRepository repository, IMapper mapper) : IProductService
{
    public async Task<Result<PagedResult<ProductResponseDto>>> GetAllAsync(PaginationParams pagination)
    {
        var pagedResult = await repository.GetAllAsync(pagination);
        
        var listProductResponseDtos = mapper.Map<IEnumerable<ProductResponseDto>>(pagedResult.Items);

        var resultDto = new PagedResult<ProductResponseDto>
        {
            Items = listProductResponseDtos,
            TotalCount = pagedResult.TotalCount,
            PageSize = pagedResult.PageSize,
            CurrentPage = pagedResult.CurrentPage,
            TotalPages = pagedResult.TotalPages
        };
        
        return Result<PagedResult<ProductResponseDto>>.Success(resultDto);
    }

    public async Task<Result<ProductResponseDto?>> GetByIdAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ProductResponseDto?>.NotFound("No products were found with this ID.");
        }
            
        var productResponseDto = mapper.Map<Product, ProductResponseDto>(product);

        return Result<ProductResponseDto?>.Success(productResponseDto);

    }

    public async Task<Result<ProductResponseDto>> AddAsync(CreateProductDto dto)
    {
        bool productExists = await repository.ExistByNameAsync(dto.Name);

        if (productExists)
        {
            return Result<ProductResponseDto>.Duplicate("Product with that name already exists.");
        }
        
        var product = mapper.Map<Product>(dto);
        
        await repository.AddAsync(product);
        
        var productReponseDto = mapper.Map<Product, ProductResponseDto>(product);
        
        return Result<ProductResponseDto>.Created(productReponseDto);
    }

    public async Task<Result<ProductResponseDto?>> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ProductResponseDto?>.NotFound("No products were found with this ID.");
        }
        
        bool isNecessaryChangeName = dto.Name != string.Empty && dto.Name != product.Name;

        if (isNecessaryChangeName)
        {
            bool nameExists = await repository.ExistByNameAsync(dto.Name);

            if (nameExists)
            {
                return Result<ProductResponseDto?>.Duplicate("Product with that name already exists.");
            }
        }

        mapper.Map(dto, product);
        
        await repository.UpdateAsync(product);
        
        var productReponseDto = mapper.Map<Product, ProductResponseDto>(product);
        
        return Result<ProductResponseDto?>.Success(productReponseDto);
    }

    public async Task<Result<ProductResponseDto?>> DeleteAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<ProductResponseDto?>.NotFound("No product were found with this ID.");
        }
        
        await repository.DeleteAsync(product);
        
        var productReponseDto = mapper.Map<Product, ProductResponseDto>(product);
        
        return Result<ProductResponseDto?>.Success(productReponseDto);
    }
}