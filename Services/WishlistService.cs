using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace BookBagaicha.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context;

        public WishlistService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WishlistDto> GetWishlistByUserIdAsync(long userId)
        {
            // Try to get the user's wishlist
            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Book)
                .ThenInclude(b => b.Authors)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            // If the user doesn't have a wishlist yet, create one
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    WishlistId = Guid.NewGuid(),
                    UserId = userId
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();

                return new WishlistDto
                {
                    WishlistId = wishlist.WishlistId,
                    UserId = wishlist.UserId,
                    Items = new List<WishlistItemDto>()
                };
            }

            // Map the wishlist to DTO
            var wishlistDto = new WishlistDto
            {
                WishlistId = wishlist.WishlistId,
                UserId = wishlist.UserId,
                Items = wishlist.WishlistItems.Select(wi => new WishlistItemDto
                {
                    WishlistItemId = wi.WishlistItemId,
                    BookId = wi.BookId,
                    BookTitle = wi.Book.Title,
                    ISBN = wi.Book.ISBN,
                    Price = wi.Book.Price,
                    Image = wi.Book.Image,
                    AddedDate = wi.AddedDate,
                    OnSale = wi.Book.OnSale,
                    SalePercentage = wi.Book.SalePercntage,
                    Category = wi.Book.Category,
                    // Combine first and last name of authors
                    Authors = wi.Book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                }).ToList()
            };

            return wishlistDto;
        }

        public async Task<WishlistItemDto> AddToWishlistAsync(long userId, Guid bookId)
        {
            // Check if the book exists
            var book = await _context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            if (book == null)
            {
                throw new ArgumentException($"Book with ID {bookId} not found.");
            }

            // Get or create the user's wishlist
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    WishlistId = Guid.NewGuid(),
                    UserId = userId
                };
                _context.Wishlists.Add(wishlist);
            }

            // Check if the book is already in the wishlist
            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(wi => wi.WishlistId == wishlist.WishlistId && wi.BookId == bookId);

            if (existingItem != null)
            {
                // Book is already in the wishlist, return the existing item
                return new WishlistItemDto
                {
                    WishlistItemId = existingItem.WishlistItemId,
                    BookId = existingItem.BookId,
                    BookTitle = book.Title,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    Image = book.Image,
                    AddedDate = existingItem.AddedDate,
                    OnSale = book.OnSale,
                    SalePercentage = book.SalePercntage,
                    Category = book.Category,
                    // Combine first and last name of authors
                    Authors = book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                };
            }

            // Add the book to the wishlist
            var wishlistItem = new WishlistItem
            {
                WishlistItemId = Guid.NewGuid(),
                WishlistId = wishlist.WishlistId,
                BookId = bookId,
                AddedDate = DateTime.Now
            };

            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();

            // Return the DTO
            var wishlistItemDto = new WishlistItemDto
            {
                WishlistItemId = wishlistItem.WishlistItemId,
                BookId = wishlistItem.BookId,
                BookTitle = book.Title,
                ISBN = book.ISBN,
                Price = book.Price,
                Image = book.Image,
                AddedDate = wishlistItem.AddedDate,
                OnSale = book.OnSale,
                SalePercentage = book.SalePercntage,
                Category = book.Category,
                // Combine first and last name of authors
                Authors = book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
            };

            return wishlistItemDto;
        }

        public async Task<bool> RemoveFromWishlistAsync(long userId, Guid wishlistItemId)
        {
            // Get the user's wishlist
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                return false;
            }

            // Find the wishlist item
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(wi => wi.WishlistItemId == wishlistItemId && wi.WishlistId == wishlist.WishlistId);

            if (wishlistItem == null)
            {
                return false;
            }

            // Remove the item
            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsBookInWishlistAsync(long userId, Guid bookId)
        {
            // Get the user's wishlist
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                return false;
            }

            // Check if the book is in the wishlist
            var isInWishlist = await _context.WishlistItems
                .AnyAsync(wi => wi.WishlistId == wishlist.WishlistId && wi.BookId == bookId);

            return isInWishlist;
        }
    }
}