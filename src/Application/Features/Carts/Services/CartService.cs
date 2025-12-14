using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Carts.DTOs;
using Application.Features.Carts.Repositories;
using Application.Features.Products.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;

namespace Application.Features.Carts.Services;

public class CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : ICartService
{
    public async Task<Result<CartResponseDto>> GetMyCartAsync(int userId)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
        {
            return Result<CartResponseDto>.Success(new CartResponseDto());
        }

        var response = mapper.Map<CartResponseDto>(cart);
        return Result<CartResponseDto>.Success(response);
    }

    public async Task<Result<CartResponseDto>> AddItemAsync(int userId, AddToCartDto dto)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            await cartRepository.AddAsync(cart);
        }

        var product = await productRepository.GetByIdAsync(dto.ProductId);
        if (product is null)
            return Result<CartResponseDto>.NotFound(ErrorMessages.ProductNotFound);

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

        if (existingItem is not null)
        {
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = dto.Quantity,
                UnitPrice = product.Price,
                Product = product // Important: Ensure navigation property is set for immediate mapping
            };

            cart.Items.Add(cartItem);
        }

        await unitOfWork.CommitAsync();

        // Re-fetch cart to ensure all Product navigation properties are populated for mapping
        var updatedCart = await cartRepository.GetByUserIdWithItemsAsync(userId);
        var response = mapper.Map<CartResponseDto>(updatedCart);
        return Result<CartResponseDto>.Success(response);
    }

    public async Task<Result<CartResponseDto>> UpdateItemQuantityAsync(int userId, int cartItemId, int quantity)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
            return Result<CartResponseDto>.NotFound(ErrorMessages.CartNotFound);

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (item is null)
            return Result<CartResponseDto>.NotFound(ErrorMessages.CartItemNotFound);

        item.Quantity = quantity;
        await unitOfWork.CommitAsync();

        var response = mapper.Map<CartResponseDto>(cart);
        return Result<CartResponseDto>.Success(response);
    }

    public async Task<Result<string>> RemoveItemAsync(int userId, int cartItemId)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
            return Result<string>.NotFound(ErrorMessages.CartNotFound);

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (item is null)
            return Result<string>.NotFound(ErrorMessages.CartItemNotFound);

        cart.Items.Remove(item);
        await unitOfWork.CommitAsync();

        return Result<string>.Success(ErrorMessages.ItemRemovedFromCart);
    }

    public async Task<Result<string>> ClearCartAsync(int userId)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null)
            return Result<string>.NotFound(ErrorMessages.CartNotFound);

        cart.Items.Clear();
        await unitOfWork.CommitAsync();

        return Result<string>.Success(ErrorMessages.CartCleared);
    }
}


