using BookBagaicha.Models;
using BookBagaicha.Models.Dto;

namespace BookBagaicha.IService
{
    public interface IWishlistService
    {
        // Get a user's wishlist
        Task<WishlistDto> GetWishlistByUserIdAsync(long userId);

        // Add a book to the wishlist
        Task<WishlistItemDto> AddToWishlistAsync(long userId, Guid bookId);

        // Remove a book from the wishlist
        Task<bool> RemoveFromWishlistAsync(long userId, Guid wishlistItemId);

        // Check if a book is in the wishlist
        Task<bool> IsBookInWishlistAsync(long userId, Guid bookId);
    }
}