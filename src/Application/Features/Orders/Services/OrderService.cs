using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Carts.Repositories;
using Application.Features.Orders.DTOs;
using Application.Features.Orders.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Orders.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IOrderService
{
    public async Task<Result<OrderResponseDto>> CreateFromCartAsync(int userId)
    {
        var cart = await cartRepository.GetByUserIdWithItemsAsync(userId);

        if (cart is null || !cart.Items.Any())
            return Result<OrderResponseDto>.NotFound(ErrorMessages.EmptyCart);

        var order = new Order
        {
            UserId = userId,
            OrderNumber = GenerateOrderNumber(),
            Status = OrderStatus.Pending,
            TotalAmount = cart.TotalAmount,
            OrderDate = DateTime.UtcNow
        };

        foreach (var cartItem in cart.Items)
        {
            // Verificação defensiva: se o produto foi deletado logicamente ou é nulo
            if (cartItem.Product is null)
                return Result<OrderResponseDto>.Failure(string.Format("{0} (ID: {1})", ErrorMessages.ProductUnavailable, cartItem.ProductId));

            order.Items.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice
            });
        }

        await orderRepository.AddAsync(order);

        cart.Items.Clear();

        await unitOfWork.CommitAsync();

        // Recarrega o pedido corretamente com todos os includes (informações do Produto) antes do mapeamento
        var savedOrder = await orderRepository.GetByIdWithItemsAsync(order.Id);

        var response = mapper.Map<OrderResponseDto>(savedOrder);
        return Result<OrderResponseDto>.Success(response);
    }

    public async Task<Result<OrderResponseDto>> GetByIdAsync(int orderId, int userId)
    {
        var order = await orderRepository.GetByIdWithItemsAsync(orderId);

        if (order is null)
            return Result<OrderResponseDto>.NotFound(ErrorMessages.OrderNotFound);

        if (order.UserId != userId)
            return Result<OrderResponseDto>.Unauthorized(ErrorMessages.AccessDenied);

        var response = mapper.Map<OrderResponseDto>(order);
        return Result<OrderResponseDto>.Success(response);
    }

    public async Task<Result<PagedResult<OrderResponseDto>>> GetMyOrdersAsync(int userId, PaginationParams pagination)
    {
        var pagedOrders = await orderRepository.GetByUserIdAsync(userId, pagination);

        var ordersDto = mapper.Map<List<OrderResponseDto>>(pagedOrders.Items);

        var result = new PagedResult<OrderResponseDto>
        {
            Items = ordersDto,
            TotalCount = pagedOrders.TotalCount,
            CurrentPage = pagedOrders.CurrentPage,
            PageSize = pagedOrders.PageSize,
            TotalPages = pagedOrders.TotalPages
        };

        return Result<PagedResult<OrderResponseDto>>.Success(result);
    }

    public async Task<Result<OrderResponseDto>> CancelOrderAsync(int orderId, int userId)
    {
        var order = await orderRepository.GetByIdWithItemsAsync(orderId);

        if (order is null)
            return Result<OrderResponseDto>.NotFound(ErrorMessages.OrderNotFound);

        if (order.UserId != userId)
            return Result<OrderResponseDto>.Unauthorized(ErrorMessages.AccessDenied);

        if (order.Status != OrderStatus.Pending)
            return Result<OrderResponseDto>.NotFound(ErrorMessages.OnlyPendingOrdersCanBeCancelled);

        order.Status = OrderStatus.Cancelled;
        await unitOfWork.CommitAsync();

        var response = mapper.Map<OrderResponseDto>(order);
        return Result<OrderResponseDto>.Success(response);
    }

    public async Task<Result<OrderResponseDto>> ConfirmPaymentAsync(int orderId, int userId)
    {
        var order = await orderRepository.GetByIdWithItemsAsync(orderId);

        if (order is null)
            return Result<OrderResponseDto>.NotFound(ErrorMessages.OrderNotFound);

        if (order.UserId != userId)
            return Result<OrderResponseDto>.Unauthorized(ErrorMessages.AccessDenied);

        if (order.Status != OrderStatus.Pending)
            return Result<OrderResponseDto>.NotFound(ErrorMessages.OnlyPendingOrdersCanBeConfirmed);

        order.Status = OrderStatus.Paid;
        await unitOfWork.CommitAsync();

        var response = mapper.Map<OrderResponseDto>(order);
        return Result<OrderResponseDto>.Success(response);
    }

    private static readonly Random _random = new();

    private static string GenerateOrderNumber()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = _random.Next(1000, 9999);
        return $"ORD-{date}-{random}";
    }
}


