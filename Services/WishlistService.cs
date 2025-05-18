using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookBagaicha.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WishlistService> _logger;

        public WishlistService(AppDbContext context, ILogger<WishlistService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WishlistDto> GetWishlistByUserIdAsync(long userId)
        {
            try
            {
                _logger.LogInformation("Getting wishlist for user {UserId}", userId);

                // get the user wishlist
                var wishlist = await _context.Wishlists
                    .Include(w => w.WishlistItems)
                    .ThenInclude(wi => wi.Book)
                    .ThenInclude(b => b.Authors)
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                // create wishlist if it doesnt exist
                if (wishlist == null)
                {
                    _logger.LogInformation("Creating new wishlist for user {UserId}", userId);

                    wishlist = new Wishlist
                    {
                        WishlistId = Guid.NewGuid(),
                        UserId = userId
                    };

                    _context.Wishlists.Add(wishlist);

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Created new wishlist with ID {WishlistId}", wishlist.WishlistId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating wishlist for user {UserId}", userId);
                        throw new Exception($"Failed to create wishlist: {ex.Message}", ex);
                    }

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
                        OnSale = wi.Book.OnSale,
                        SalePercentage = wi.Book.SalePercntage,
                        Category = wi.Book.Category,
                        // Combine first and last name of authors
                        Authors = wi.Book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                    }).ToList()
                };

                return wishlistDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wishlist for user {UserId}", userId);
                throw new Exception($"Failed to retrieve wishlist: {ex.Message}", ex);
            }
        }

        public async Task<WishlistItemDto> AddToWishlistAsync(long userId, Guid bookId)
        {
            try
            {
                _logger.LogInformation("Adding book {BookId} to wishlist for user {UserId}", bookId, userId);

                // Verify the book exists
                var book = await _context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(b => b.BookId == bookId);

                if (book == null)
                {
                    _logger.LogWarning("Book with ID {BookId} not found", bookId);
                    throw new ArgumentException($"Book with ID {bookId} not found.");
                }

                // Get or create the user wishlist
                var wishlist = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wishlist == null)
                {
                    _logger.LogInformation("Creating new wishlist for user {UserId}", userId);

                    wishlist = new Wishlist
                    {
                        WishlistId = Guid.NewGuid(),
                        UserId = userId
                    };

                    _context.Wishlists.Add(wishlist);

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Created new wishlist with ID {WishlistId}", wishlist.WishlistId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating wishlist for user {UserId}", userId);
                        throw new Exception($"Failed to create wishlist: {ex.Message}", ex);
                    }
                }

                // Check if the book is already in the wishlist
                var existingItem = await _context.WishlistItems
                    .FirstOrDefaultAsync(wi => wi.WishlistId == wishlist.WishlistId && wi.BookId == bookId);

                if (existingItem != null)
                {
                    _logger.LogInformation("Book {BookId} is already in wishlist {WishlistId}", bookId, wishlist.WishlistId);

                    // return the existing item if book is in wishlist
                    return new WishlistItemDto
                    {
                        WishlistItemId = existingItem.WishlistItemId,
                        BookId = existingItem.BookId,
                        BookTitle = book.Title,
                        ISBN = book.ISBN,
                        Price = book.Price,
                        Image = book.Image,
                        OnSale = book.OnSale,
                        SalePercentage = book.SalePercntage,
                        Category = book.Category,
                        // Combine first and last name of authors
                        Authors = book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                    };
                }

                // Create a new wishlist item
                var wishlistItem = new WishlistItem
                {
                    WishlistItemId = Guid.NewGuid(),
                    WishlistId = wishlist.WishlistId,
                    BookId = bookId,
                   
                };

                _logger.LogInformation("Creating wishlist item: {WishlistItem}",
                    new { wishlistItem.WishlistItemId, wishlistItem.WishlistId, wishlistItem.BookId });

                // separate transaction for adding the item
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _context.WishlistItems.Add(wishlistItem);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Successfully added book {BookId} to wishlist", bookId);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error adding book {BookId} to wishlist: {ErrorMessage}",
                            bookId, ex.InnerException?.Message ?? ex.Message);

                        throw new Exception($"Failed to add book to wishlist: {ex.Message}", ex);
                    }
                }

                // Return the DTO
                var wishlistItemDto = new WishlistItemDto
                {
                    WishlistItemId = wishlistItem.WishlistItemId,
                    BookId = wishlistItem.BookId,
                    BookTitle = book.Title,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    Image = book.Image,
                    
                    OnSale = book.OnSale,
                    SalePercentage = book.SalePercntage,
                    Category = book.Category,
                    // Combine first and last name of authors
                    Authors = book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                };

                return wishlistItemDto;
            }
            catch (ArgumentException)
            {
                // book not found
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding book {BookId} to wishlist: {Message}",
                    bookId, ex.Message);
                throw new Exception($"Failed to add book to wishlist: {ex.Message}", ex);
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(long userId, Guid wishlistItemId)
        {
            try
            {
                _logger.LogInformation("Removing wishlist item {WishlistItemId} for user {UserId}", wishlistItemId, userId);

                // Get the user wishlist
                var wishlist = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wishlist == null)
                {
                    _logger.LogWarning("Wishlist not found for user {UserId}", userId);
                    return false;
                }

                // Find the wishlist item
                var wishlistItem = await _context.WishlistItems
                    .FirstOrDefaultAsync(wi => wi.WishlistItemId == wishlistItemId && wi.WishlistId == wishlist.WishlistId);

                if (wishlistItem == null)
                {
                    _logger.LogWarning("Wishlist item {WishlistItemId} not found in wishlist {WishlistId}",
                        wishlistItemId, wishlist.WishlistId);
                    return false;
                }

                // Remove the item
                _context.WishlistItems.Remove(wishlistItem);

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully removed wishlist item {WishlistItemId}", wishlistItemId);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing wishlist item {WishlistItemId}: {Message}",
                        wishlistItemId, ex.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error removing wishlist item {WishlistItemId}: {Message}",
                    wishlistItemId, ex.Message);
                return false;
            }
        }

        public async Task<bool> IsBookInWishlistAsync(long userId, Guid bookId)
        {
            try
            {
                _logger.LogInformation("Checking if book {BookId} is in wishlist for user {UserId}", bookId, userId);

                // Get the user wishlist
                var wishlist = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wishlist == null)
                {
                    _logger.LogInformation("Wishlist not found for user {UserId}", userId);
                    return false;
                }

                // Check if the book is in the wishlist
                var isInWishlist = await _context.WishlistItems
                    .AnyAsync(wi => wi.WishlistId == wishlist.WishlistId && wi.BookId == bookId);

                _logger.LogInformation("Book {BookId} is{NotInWishlist} in wishlist for user {UserId}",
                    bookId, isInWishlist ? "" : " not", userId);

                return isInWishlist;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if book {BookId} is in wishlist: {Message}",
                    bookId, ex.Message);
                return false;
            }
        }
    }
}