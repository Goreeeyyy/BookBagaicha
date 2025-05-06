using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace BookBagaicha.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartDto> GetCartByUserIdAsync(string userId)
        {
            // Convert string userId to long
            if (!long.TryParse(userId, out long userIdLong))
            {
                throw new ArgumentException("Invalid user ID format");
            }

            // Try to get the user's cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .ThenInclude(b => b.Authors)
                .FirstOrDefaultAsync(c => c.UserId == userIdLong);

            // If the user doesn't have a cart yet, create one
            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    UserId = userIdLong,
                    CartTotal = 0
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                return new CartDto
                {
                    CartId = cart.CartId,
                    UserId = userId,
                    CartTotal = 0,
                    Items = new List<CartItemDto>()
                };
            }

            // Calculate the cart total
            decimal cartTotal = 0;
            var cartItemDtos = new List<CartItemDto>();

            foreach (var item in cart.CartItems)
            {
                decimal itemPrice = item.Price;

                // If the book is on sale, apply the discount
                if (item.Book.OnSale && item.Book.SalePercntage > 0)
                {
                    decimal discountAmount = (item.Price * item.Book.SalePercntage) / 100;
                    itemPrice = item.Price - discountAmount;
                }

                cartTotal += itemPrice * item.Quantity;

                cartItemDtos.Add(new CartItemDto
                {
                    CartItemId = item.CartItemId,
                    BookId = item.BookId,
                    BookTitle = item.Book.Title,
                    ISBN = item.Book.ISBN,
                    Price = item.Price,
                    Image = item.Book.Image,
                    Quantity = item.Quantity,
                    OnSale = item.Book.OnSale,
                    SalePercentage = item.Book.SalePercntage,
                    Category = item.Book.Category,
                    // Combine first and last name of authors
                    Authors = item.Book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                });
            }

            // Update cart total if it has changed
            if (cart.CartTotal != cartTotal)
            {
                cart.CartTotal = cartTotal;
                await _context.SaveChangesAsync();
            }

            // Map the cart to DTO
            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                UserId = userId,
                CartTotal = cartTotal,
                Items = cartItemDtos
            };

            return cartDto;
        }

        public async Task<CartItemDto> AddToCartAsync(string userId, Guid bookId, int quantity = 1)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero");
            }

            // Convert string userId to long
            if (!long.TryParse(userId, out long userIdLong))
            {
                throw new ArgumentException("Invalid user ID format");
            }

            // Check if the book exists
            var book = await _context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            if (book == null)
            {
                throw new ArgumentException($"Book with ID {bookId} not found.");
            }

            // Get or create the user's cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userIdLong);

            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    UserId = userIdLong,
                    CartTotal = 0
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if the book is already in the cart
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.BookId == bookId);

            if (existingItem != null)
            {
                // Update the quantity if the book is already in the cart
                existingItem.Quantity += quantity;
                _context.CartItems.Update(existingItem);

                // Recalculate cart total
                await CalculateCartTotalAsync(userId);

                await _context.SaveChangesAsync();

                // Return the updated item
                return new CartItemDto
                {
                    CartItemId = existingItem.CartItemId,
                    BookId = existingItem.BookId,
                    BookTitle = book.Title,
                    ISBN = book.ISBN,
                    Price = existingItem.Price,
                    Image = book.Image,
                    Quantity = existingItem.Quantity,
                    OnSale = book.OnSale,
                    SalePercentage = book.SalePercntage,
                    Category = book.Category,
                    // Combine first and last name of authors
                    Authors = book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                };
            }

            // Create a new cart item
            var cartItem = new CartItem
            {
                CartItemId = Guid.NewGuid(),
                CartId = cart.CartId,
                BookId = bookId,
                Quantity = quantity,
                Price = book.Price // Store current book price
            };

            _context.CartItems.Add(cartItem);

            // Recalculate cart total
            await CalculateCartTotalAsync(userId);

            await _context.SaveChangesAsync();

            // Return the DTO
            var cartItemDto = new CartItemDto
            {
                CartItemId = cartItem.CartItemId,
                BookId = cartItem.BookId,
                BookTitle = book.Title,
                ISBN = book.ISBN,
                Price = cartItem.Price,
                Image = book.Image,
                Quantity = cartItem.Quantity,
                OnSale = book.OnSale,
                SalePercentage = book.SalePercntage,
                Category = book.Category,
                // Combine first and last name of authors
                Authors = book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
            };

            return cartItemDto;
        }

        public async Task<CartItemDto> UpdateCartItemAsync(string userId, Guid cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                // If quantity is 0 or negative, remove the item
                await RemoveFromCartAsync(userId, cartItemId);
                return null;
            }

            // Convert string userId to long
            if (!long.TryParse(userId, out long userIdLong))
            {
                throw new ArgumentException("Invalid user ID format");
            }

            // Get the user's cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userIdLong);

            if (cart == null)
            {
                throw new ArgumentException("Cart not found for this user.");
            }

            // Find the cart item
            var cartItem = await _context.CartItems
                .Include(ci => ci.Book)
                .ThenInclude(b => b.Authors)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.CartId == cart.CartId);

            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found.");
            }

            // Update the quantity
            cartItem.Quantity = quantity;
            _context.CartItems.Update(cartItem);

            // Recalculate cart total
            await CalculateCartTotalAsync(userId);

            await _context.SaveChangesAsync();

            // Return the updated item
            return new CartItemDto
            {
                CartItemId = cartItem.CartItemId,
                BookId = cartItem.BookId,
                BookTitle = cartItem.Book.Title,
                ISBN = cartItem.Book.ISBN,
                Price = cartItem.Price,
                Image = cartItem.Book.Image,
                Quantity = cartItem.Quantity,
                OnSale = cartItem.Book.OnSale,
                SalePercentage = cartItem.Book.SalePercntage,
                Category = cartItem.Book.Category,
                // Combine first and last name of authors
                Authors = cartItem.Book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
            };
        }

        public async Task<bool> RemoveFromCartAsync(string userId, Guid cartItemId)
        {
            // Convert string userId to long
            if (!long.TryParse(userId, out long userIdLong))
            {
                throw new ArgumentException("Invalid user ID format");
            }

            // Get the user's cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userIdLong);

            if (cart == null)
            {
                return false;
            }

            // Find the cart item
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.CartId == cart.CartId);

            if (cartItem == null)
            {
                return false;
            }

            // Remove the item
            _context.CartItems.Remove(cartItem);

            // Recalculate cart total
            await CalculateCartTotalAsync(userId);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            // Convert string userId to long
            if (!long.TryParse(userId, out long userIdLong))
            {
                throw new ArgumentException("Invalid user ID format");
            }

            // Get the user's cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userIdLong);

            if (cart == null)
            {
                return false;
            }

            // Remove all items from the cart
            _context.CartItems.RemoveRange(cart.CartItems);

            // Reset the cart total
            cart.CartTotal = 0;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> CalculateCartTotalAsync(string userId)
        {
            // Convert string userId to long
            if (!long.TryParse(userId, out long userIdLong))
            {
                throw new ArgumentException("Invalid user ID format");
            }

            // Get the user's cart with items
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userIdLong);

            if (cart == null)
            {
                return 0;
            }

            // Calculate the total
            decimal total = 0;
            foreach (var item in cart.CartItems)
            {
                decimal itemPrice = item.Price;

                // Apply discount if the book is on sale
                if (item.Book.OnSale && item.Book.SalePercntage > 0)
                {
                    decimal discountAmount = (item.Price * item.Book.SalePercntage) / 100;
                    itemPrice = item.Price - discountAmount;
                }

                total += itemPrice * item.Quantity;
            }

            // Update the cart total
            cart.CartTotal = total;
            await _context.SaveChangesAsync();

            return total;
        }
    }
}