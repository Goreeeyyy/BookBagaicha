using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookBagaicha.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(AppDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartDto> GetCartByUserIdAsync(long userId)
        {
            try
            {
                _logger.LogInformation("Getting cart for user {UserId}", userId);

                // get the user cart with all items
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Book)
                    .ThenInclude(b => b.Authors)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                // create cart if it doesnt exist
                if (cart == null)
                {
                    _logger.LogInformation("Creating new cart for user {UserId}", userId);

                    cart = new Cart
                    {
                        CartId = Guid.NewGuid(),
                        UserId = userId,
                        CartTotal = 0
                    };

                    _context.Carts.Add(cart);

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Created new cart with ID {CartId}", cart.CartId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating cart for user {UserId}", userId);
                        throw new Exception($"Failed to create cart: {ex.Message}", ex);
                    }

                    return new CartDto
                    {
                        CartId = cart.CartId,
                        UserId = cart.UserId,
                        Items = new List<CartItemDto>(),
                        CartTotal = 0
                    };
                }

                // recalculate cart total
                cart.CartTotal = cart.CartItems.Sum(ci => ci.Book.Price * ci.Quantity);
                await _context.SaveChangesAsync();

                // map the cart to DTO
                var cartDto = MapCartToDto(cart);
                return cartDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart for user {UserId}", userId);
                throw new Exception($"Failed to retrieve cart: {ex.Message}", ex);
            }
        }

        public async Task<CartDto> GetCartByIdAsync(Guid cartId)
        {
            try
            {
                _logger.LogInformation("Getting cart with ID {CartId}", cartId);

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Book)
                    .ThenInclude(b => b.Authors)
                    .FirstOrDefaultAsync(c => c.CartId == cartId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart with ID {CartId} not found", cartId);
                    throw new ArgumentException($"Cart with ID {cartId} not found.");
                }

                // recalculate cart total
                cart.CartTotal = cart.CartItems.Sum(ci => ci.Book.Price * ci.Quantity);
                await _context.SaveChangesAsync();

                // map the cart to DTO
                var cartDto = MapCartToDto(cart);
                return cartDto;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart with ID {CartId}", cartId);
                throw new Exception($"Failed to retrieve cart: {ex.Message}", ex);
            }
        }

        public async Task<CartItemDto> AddToCartAsync(Guid cartId, Guid bookId, int quantity)
        {
            try
            {
                _logger.LogInformation("Adding book {BookId} to cart {CartId} with quantity {Quantity}", bookId, cartId, quantity);

                // Verify the cart exists
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CartId == cartId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart with ID {CartId} not found", cartId);
                    throw new ArgumentException($"Cart with ID {cartId} not found.");
                }

                // Verify the book exists
                var book = await _context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(b => b.BookId == bookId);

                if (book == null)
                {
                    _logger.LogWarning("Book with ID {BookId} not found", bookId);
                    throw new ArgumentException($"Book with ID {bookId} not found.");
                }

                // Check if the book is already in the cart
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.BookId == bookId);

                if (existingItem != null)
                {
                    // Update quantity if the book is already in the cart
                    existingItem.Quantity += quantity;
                    _context.CartItems.Update(existingItem);
                    _logger.LogInformation("Updated quantity for book {BookId} in cart {CartId} to {Quantity}",
                        bookId, cartId, existingItem.Quantity);
                }
                else
                {
                    // Add new cart item
                    var cartItem = new CartItem
                    {
                        CartItemId = Guid.NewGuid(),
                        CartId = cartId,
                        BookId = bookId,
                        Quantity = quantity
                    };

                    _context.CartItems.Add(cartItem);
                    existingItem = cartItem;
                    _logger.LogInformation("Added new cart item for book {BookId} to cart {CartId}", bookId, cartId);
                }

                // Update cart total
                cart.CartTotal = cart.CartItems.Sum(ci => ci.Book.Price * ci.Quantity) + (book.Price * quantity);
                _logger.LogInformation("Updated cart total to {CartTotal}", cart.CartTotal);

                await _context.SaveChangesAsync();

                var cartItemDto = new CartItemDto
                {
                    CartItemId = existingItem.CartItemId,
                    BookId = bookId,
                    BookTitle = book.Title,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    Image = book.Image,
                    Quantity = existingItem.Quantity,
                    OnSale = book.OnSale,
                    SalePercentage = book.SalePercntage,
                    Authors = book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                };

                return cartItemDto;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding book {BookId} to cart {CartId}: {Message}", bookId, cartId, ex.Message);
                throw new Exception($"Failed to add book to cart: {ex.Message}", ex);
            }
        }

        public async Task<CartItemDto> UpdateCartItemQuantityAsync(Guid cartId, Guid bookId, int quantity)
        {
            try
            {
                _logger.LogInformation("Updating quantity of book {BookId} in cart {CartId} to {Quantity}",
                    bookId, cartId, quantity);

                // Verify the cart exists
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CartId == cartId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart with ID {CartId} not found", cartId);
                    throw new ArgumentException($"Cart with ID {cartId} not found.");
                }

                // Verify the book exists
                var book = await _context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(b => b.BookId == bookId);

                if (book == null)
                {
                    _logger.LogWarning("Book with ID {BookId} not found", bookId);
                    throw new ArgumentException($"Book with ID {bookId} not found.");
                }

                // Find the cart item
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.BookId == bookId);

                if (cartItem == null)
                {
                    _logger.LogWarning("Book {BookId} not found in cart {CartId}", bookId, cartId);
                    throw new ArgumentException($"Book with ID {bookId} not found in cart.");
                }

                // Update quantity
                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);

                // recalculate cart total
                cart.CartTotal = cart.CartItems.Sum(ci => ci.Book.Price * ci.Quantity);
                _logger.LogInformation("Updated cart total to {CartTotal}", cart.CartTotal);

                await _context.SaveChangesAsync();

                // return the DTO
                var cartItemDto = new CartItemDto
                {
                    CartItemId = cartItem.CartItemId,
                    BookId = bookId,
                    BookTitle = book.Title,
                    ISBN = book.ISBN,
                    Price = book.Price,
                    Image = book.Image,
                    Quantity = quantity,
                    OnSale = book.OnSale,
                    SalePercentage = book.SalePercntage,
                    Authors = book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                };

                return cartItemDto;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item: {Message}", ex.Message);
                throw new Exception($"Failed to update cart item: {ex.Message}", ex);
            }
        }

        public async Task<bool> RemoveFromCartAsync(Guid cartId, Guid bookId)
        {
            try
            {
                _logger.LogInformation("Removing book {BookId} from cart {CartId}", bookId, cartId);

                // Verify the cart exists
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Book)
                    .FirstOrDefaultAsync(c => c.CartId == cartId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart with ID {CartId} not found", cartId);
                    return false;
                }

                // Find the cart item
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.BookId == bookId);

                if (cartItem == null)
                {
                    _logger.LogWarning("Book {BookId} not found in cart {CartId}", bookId, cartId);
                    return false;
                }

                // remove the item
                _context.CartItems.Remove(cartItem);

                // recalculate cart total
                cart.CartTotal = cart.CartItems
                    .Where(ci => ci.BookId != bookId)
                    .Sum(ci => ci.Book.Price * ci.Quantity);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully removed book {BookId} from cart {CartId}", bookId, cartId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing book {BookId} from cart {CartId}: {Message}",
                    bookId, cartId, ex.Message);
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            try
            {
                _logger.LogInformation("Clearing cart {CartId}", cartId);

                // Verify the cart exists
                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.CartId == cartId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart with ID {CartId} not found", cartId);
                    return false;
                }

                // Get all cart items
                var cartItems = await _context.CartItems
                    .Where(ci => ci.CartId == cartId)
                    .ToListAsync();

                // remove all items
                _context.CartItems.RemoveRange(cartItems);

                // reset cart total
                cart.CartTotal = 0;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully cleared cart {CartId}", cartId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart {CartId}: {Message}", cartId, ex.Message);
                return false;
            }
        }

        // Helper method to map Cart to CartDto
        private CartDto MapCartToDto(Cart cart)
        {
            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CartTotal = cart.CartTotal,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    BookId = ci.BookId,
                    BookTitle = ci.Book.Title,
                    ISBN = ci.Book.ISBN,
                    Price = ci.Book.Price,
                    Image = ci.Book.Image,
                    Quantity = ci.Quantity,
                    OnSale = ci.Book.OnSale,
                    SalePercentage = ci.Book.SalePercntage,
                    Authors = ci.Book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                }).ToList()
            };

            return cartDto;
        }
    }
}