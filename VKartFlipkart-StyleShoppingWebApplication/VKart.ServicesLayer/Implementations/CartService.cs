using VKart.ModelsLayer.DTOs;
using VKart.ModelsLayer.Entities;
using VKart.RepositoriesLayer.Interfaces;
using VKart.ServicesLayer.Common;
using VKart.ServicesLayer.Interfaces;

namespace VKart.ServicesLayer.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;

    public CartService(ICartRepository cartRepo, IProductRepository productRepo)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
    }

    public async Task<ServiceResult<CartDto>> GetCartAsync(string userId)
    {
        var cart = await _cartRepo.GetByUserIdWithItemsAsync(userId);
        if (cart is null)
        {
            // Create empty cart lazily
            cart = new Cart { UserId = userId };
            await _cartRepo.AddAsync(cart);
            await _cartRepo.SaveChangesAsync();
        }
        return ServiceResult<CartDto>.Ok(Map(cart));
    }

    public async Task<ServiceResult<CartDto>> AddToCartAsync(string userId, AddToCartDto dto)
    {
        var product = await _productRepo.GetByIdAsync(dto.ProductId);
        if (product is null || !product.IsActive)
            return ServiceResult<CartDto>.Fail("Product not available.");

        if (dto.Quantity < 1) dto.Quantity = 1;

        var cart = await _cartRepo.GetByUserIdWithItemsAsync(userId);
        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            await _cartRepo.AddAsync(cart);
            await _cartRepo.SaveChangesAsync();
        }

        var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
        var newQty = (existing?.Quantity ?? 0) + dto.Quantity;

        if (newQty > product.StockQuantity)
            return ServiceResult<CartDto>.Fail(
                $"Only {product.StockQuantity} item(s) available for {product.ProductName}.");

        if (existing is not null)
        {
            existing.Quantity = newQty;
            _cartRepo.Update(cart);
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                CartId = cart.CartId,
                ProductId = product.ProductId,
                Quantity = dto.Quantity
            });
            _cartRepo.Update(cart);
        }

        await _cartRepo.SaveChangesAsync();

        // Reload fresh
        var fresh = await _cartRepo.GetByUserIdWithItemsAsync(userId);
        return ServiceResult<CartDto>.Ok(Map(fresh!));
    }

    public async Task<ServiceResult<CartDto>> UpdateQuantityAsync(string userId, int cartItemId, int quantity)
    {
        var cart = await _cartRepo.GetByUserIdWithItemsAsync(userId);
        if (cart is null) return ServiceResult<CartDto>.Fail("Cart not found.");

        var item = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
        if (item is null) return ServiceResult<CartDto>.Fail("Cart item not found.");

        if (quantity < 1)
            return ServiceResult<CartDto>.Fail("Quantity must be at least 1.");

        if (item.Product is null || quantity > item.Product.StockQuantity)
            return ServiceResult<CartDto>.Fail(
                $"Only {item.Product?.StockQuantity ?? 0} item(s) available.");

        item.Quantity = quantity;
        _cartRepo.Update(cart);
        await _cartRepo.SaveChangesAsync();

        var fresh = await _cartRepo.GetByUserIdWithItemsAsync(userId);
        return ServiceResult<CartDto>.Ok(Map(fresh!));
    }

    public async Task<ServiceResult<CartDto>> RemoveItemAsync(string userId, int cartItemId)
    {
        var cart = await _cartRepo.GetByUserIdWithItemsAsync(userId);
        if (cart is null) return ServiceResult<CartDto>.Fail("Cart not found.");

        var item = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
        if (item is null) return ServiceResult<CartDto>.Fail("Cart item not found.");

        cart.CartItems.Remove(item);
        _cartRepo.Update(cart);
        await _cartRepo.SaveChangesAsync();

        var fresh = await _cartRepo.GetByUserIdWithItemsAsync(userId);
        return ServiceResult<CartDto>.Ok(Map(fresh!));
    }

    public async Task<ServiceResult> ClearCartAsync(string userId)
    {
        var cart = await _cartRepo.GetByUserIdWithItemsAsync(userId);
        if (cart is null) return ServiceResult.Ok();

        cart.CartItems.Clear();
        _cartRepo.Update(cart);
        await _cartRepo.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    private static CartDto Map(Cart cart)
    {
        var items = cart.CartItems.Select(ci => new CartItemDto
        {
            CartItemId = ci.CartItemId,
            ProductId = ci.ProductId,
            ProductName = ci.Product?.ProductName ?? "",
            ImageUrl = ci.Product?.ImageUrl,
            UnitPrice = ci.Product?.Price ?? 0,
            Quantity = ci.Quantity,
            LineTotal = (ci.Product?.Price ?? 0) * ci.Quantity,
            AvailableStock = ci.Product?.StockQuantity ?? 0
        }).ToList();

        return new CartDto
        {
            CartId = cart.CartId,
            Items = items,
            SubTotal = items.Sum(i => i.LineTotal),
            TotalItems = items.Sum(i => i.Quantity)
        };
    }
}