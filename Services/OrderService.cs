using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookBagaicha.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext context, ICartService cartService, ILogger<OrderService> logger)
        {
            _context = context;
            _cartService = cartService;
            _logger = logger;
        }

        public async Task<List<OrderSummaryDto>> GetAllOrdersAsync(long userId)
        {
            try
            {
                _logger.LogInformation("Getting all orders for user {UserId}", userId);

                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return orders.Select(o => new OrderSummaryDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalPrice = o.TotalPrice,
                    ClaimCode = o.ClaimCode,
                    ItemCount = o.OrderItems.Count
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetSuccessfulOrderCountAsync(long userId)
        {
            try
            {
                _logger.LogInformation("Getting successful order count for user {UserId}", userId);

                return await _context.Orders
                    .CountAsync(o => o.UserId == userId && o.Status == "Confirmed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting successful order count for user {UserId}", userId);
                return 0;
            }
        }

        public async Task<OrderDto> GetOrderDetailsAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Getting details for order {OrderId}", orderId);

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .ThenInclude(b => b.Authors)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found", orderId);
                    throw new ArgumentException($"Order with ID {orderId} not found.");
                }

                return new OrderDto
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalPrice = order.TotalPrice,
                    ClaimCode = order.ClaimCode,
                    AppliedDiscount = order.AppliedDiscount,
                    Items = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        OrderItemId = oi.OrderItemId,
                        BookId = oi.BookId,
                        BookTitle = oi.Book.Title,
                        ISBN = oi.Book.ISBN,
                        Image = oi.Book.Image,
                        Quantity = oi.Quantity,
                        PriceAtPurchase = oi.PriceAtPurchase,
                        Authors = oi.Book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList()
                    }).ToList()
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details for order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<OrderDto> PlaceOrderAsync(long userId, PlaceOrderRequest request, string claimCode)
        {
            try
            {
                _logger.LogInformation("Placing order for user {UserId} from cart {CartId}", userId, request.CartId);

                var cart = await _cartService.GetCartByIdAsync(request.CartId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart with ID {CartId} not found", request.CartId);
                    throw new ArgumentException("Cart not found.");
                }

                if (cart.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to place order using cart {CartId} which belongs to another user", userId, request.CartId);
                    throw new UnauthorizedAccessException("You are not authorized to place an order using this cart");
                }

                if (cart.Items == null || !cart.Items.Any())
                {
                    _logger.LogWarning("User {UserId} attempted to place order with an empty cart", userId);
                    throw new InvalidOperationException("Cannot place order with an empty cart");
                }

                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Confirmed",
                    TotalPrice = cart.CartTotal,
                    ClaimCode = claimCode,
                    AppliedDiscount = request.AppliedDiscount ?? 0,
                    ConfirmationEmailSent = false,
                    OrderItems = new List<OrderItem>()
                };

                if (request.AppliedDiscount.HasValue && request.AppliedDiscount.Value > 0)
                {
                    order.TotalPrice -= request.AppliedDiscount.Value;
                }

                foreach (var cartItem in cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderItemId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        BookId = cartItem.BookId,
                        Quantity = cartItem.Quantity,
                        PriceAtPurchase = cartItem.Price
                    };

                    order.OrderItems.Add(orderItem);

                    var book = await _context.Books.FindAsync(cartItem.BookId);
                    if (book != null)
                    {
                        book.Quantity -= cartItem.Quantity;
                        _context.Books.Update(book);
                        _logger.LogInformation("Updated quantity for book {BookId}. New quantity: {NewQuantity}", book.BookId, book.Quantity);
                    }
                    else
                    {
                        _logger.LogWarning("Book with ID {BookId} not found while placing order.", cartItem.BookId);
                        // Consider how to handle this scenario: maybe log a warning, skip the update, or throw an exception
                    }
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Orders.Add(order);

                    await _cartService.ClearCartAsync(request.CartId);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error saving order for user {UserId}", userId);
                    throw;
                }

                return await GetOrderDetailsAsync(order.OrderId);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not UnauthorizedAccessException && ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Unexpected error while placing order for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, long userId)
        {
            try
            {
                _logger.LogInformation("Cancelling order {OrderId} for user {UserId}", orderId, userId);

                var order = await _context.Orders
                    .Include(o => o.OrderItems) // Ensure OrderItems are loaded
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found", orderId);
                    throw new ArgumentException($"Order with ID {orderId} not found.");
                }

                if (order.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to cancel order {OrderId} which belongs to another user", userId, orderId);
                    throw new UnauthorizedAccessException("You are not authorized to cancel this order");
                }

                if (order.Status == "Cancelled")
                {
                    _logger.LogInformation("Order {OrderId} is already cancelled", orderId);
                    return true;
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    order.Status = "Cancelled";
                    _context.Orders.Update(order);

                    // Add back the quantity of the books
                    foreach (var orderItem in order.OrderItems)
                    {
                        var book = await _context.Books.FindAsync(orderItem.BookId);
                        if (book != null)
                        {
                            book.Quantity += orderItem.Quantity;
                            _context.Books.Update(book);
                            _logger.LogInformation("Added back {Quantity} for book {BookId}. New quantity: {NewQuantity}", orderItem.Quantity, book.BookId, book.Quantity);
                        }
                        else
                        {
                            _logger.LogWarning("Book with ID {BookId} not found while cancelling order {OrderId}.", orderItem.BookId, orderId);
                            // Consider how to handle this scenario: log a warning, or potentially an error depending on your requirements
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Successfully cancelled order {OrderId}", orderId);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                    return false;
                }
            }
            catch (ArgumentException) { throw; }
            catch (UnauthorizedAccessException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while cancelling order {OrderId}", orderId);
                return false;
            }
        }
    }
}
