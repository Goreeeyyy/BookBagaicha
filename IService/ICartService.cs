using BookBagaicha.Models;
using BookBagaicha.Models.Dto;

namespace BookBagaicha.IService
{
    public interface ICartService
    {
        // Get a user's cart or create a new one if it doesn't exist
        Task<CartDto> GetCartByUserIdAsync(long userId);

        // Get cart by cart ID
        Task<CartDto> GetCartByIdAsync(Guid cartId);

        // Add a book to the cart
        Task<CartItemDto> AddToCartAsync(Guid cartId, Guid bookId, int quantity);

        // Update item quantity in cart
        Task<CartItemDto> UpdateCartItemQuantityAsync(Guid cartId, Guid bookId, int quantity);

        // Remove an item from cart
        Task<bool> RemoveFromCartAsync(Guid cartId, Guid bookId);

        // Clear all items from cart
        Task<bool> ClearCartAsync(Guid cartId);
    }
}