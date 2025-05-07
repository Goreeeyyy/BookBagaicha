using BookBagaicha.Models;
using BookBagaicha.Models.Dto;

namespace BookBagaicha.IService
{
    public interface ICartService
    {
        // Get a user's cart
        Task<CartDto> GetCartByUserIdAsync(string userId);

        // Add a book to the cart
        Task<CartItemDto> AddToCartAsync(string userId, Guid bookId, int quantity = 1);

        // Update cart item quantity
        Task<CartItemDto> UpdateCartItemAsync(string userId, Guid cartItemId, int quantity);

        // Remove a book from the cart
        Task<bool> RemoveFromCartAsync(string userId, Guid cartItemId);

        // Clear the cart
        Task<bool> ClearCartAsync(string userId);

        // Calculate cart total
        Task<decimal> CalculateCartTotalAsync(string userId);
    }
}