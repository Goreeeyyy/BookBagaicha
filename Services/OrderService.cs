using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Identity;
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
        private readonly NotificationWebSocketHandler _notificationHandler;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public OrderService(
            AppDbContext context,
            ICartService cartService,
            ILogger<OrderService> logger,
            NotificationWebSocketHandler notificationHandler,
            UserManager<User> userManager,
            IEmailService emailService)
        {
            _context = context;
            _cartService = cartService;
            _logger = logger;
            _notificationHandler = notificationHandler;
            _userManager = userManager;
            _emailService = emailService;
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

                // Count completed orders
                return await _context.Orders
                    .CountAsync(o => o.UserId == userId && o.Status == "Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting successful order count for user {UserId}", userId);
                return 0;
            }
        }

        // Method to get the count of completed orders since the last loyalty discount
        private async Task<int> GetCompletedOrdersSinceLastLoyaltyDiscountAsync(long userId)
        {
            try
            {
                // Get the most recent order where a loyalty discount was applied
                var lastLoyaltyOrder = await _context.Orders
                    .Where(o => o.UserId == userId && o.AppliedDiscountIsLoyalty)
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefaultAsync();

                if (lastLoyaltyOrder == null)
                {
                    // If no loyalty discount has ever been applied, count all completed orders
                    return await _context.Orders
                        .CountAsync(o => o.UserId == userId && o.Status == "Completed");
                }
                else
                {
                    // If a loyalty discount has been applied before, count completed orders since that date
                    return await _context.Orders
                        .CountAsync(o => o.UserId == userId &&
                                  o.Status == "Completed" &&
                                  o.OrderDate > lastLoyaltyOrder.OrderDate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting completed orders since last loyalty discount for user {UserId}", userId);
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

        public async Task<OrderDto> PlaceOrderAsync(long userId, PlaceOrderRequest request)
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
                    ClaimCode = "ORDER-" + Guid.NewGuid().ToString().Substring(0, 8),
                    AppliedDiscount = request.AppliedDiscount ?? 0,
                    ConfirmationEmailSent = false,
                    AppliedDiscountIsLoyalty = false,
                    OrderItems = new List<OrderItem>()
                };

                // Get completed orders since last loyalty discount
                int completedSinceLastDiscount = await GetCompletedOrdersSinceLastLoyaltyDiscountAsync(userId);
                _logger.LogInformation("User {UserId} has {CompletedCount} completed orders since last loyalty discount",
                    userId, completedSinceLastDiscount);

                // Apply 10% loyalty discount if completed EXACTLY 10 orders since last discount
                if (completedSinceLastDiscount >= 10)
                {
                    decimal discountAmount = Math.Round(order.TotalPrice * 0.1m, 2);
                    order.AppliedDiscount = discountAmount;
                    order.TotalPrice -= discountAmount;
                    order.AppliedDiscountIsLoyalty = true;
                    _logger.LogInformation("Applied 10% loyalty discount of {DiscountAmount} for user {UserId} on order {OrderId} after {CompletedCount} completed orders",
                        discountAmount, userId, order.OrderId, completedSinceLastDiscount);
                }
                else if (request.AppliedDiscount.HasValue && request.AppliedDiscount.Value > 0)
                {
                    // Apply any other discount that might have been passed (like quantity discount)
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
                    }
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Orders.Add(order);
                    await _cartService.ClearCartAsync(request.CartId);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Get user details for email
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        await SendOrderConfirmationEmailAsync(user.Email, order);
                    }

                    var staffUsers = await _userManager.GetUsersInRoleAsync("Admin");
                    foreach (var staffUser in staffUsers)
                    {
                        _logger.LogInformation("Sending 'new_order' notification to staff user {StaffUserId} for order {OrderId}.", staffUser.Id, order.OrderId);
                        await _notificationHandler.SendNotificationToUserAsync(
                            staffUser.Id,
                            new
                            {
                                type = "new_order",
                                orderId = order.OrderId,
                                orderDate = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                customerUserId = userId,
                                totalPrice = order.TotalPrice
                            });
                        _logger.LogInformation("'new_order' notification sent successfully to staff user {StaffUserId} for order {OrderId}.", staffUser.Id, order.OrderId);
                    }

                    await _notificationHandler.SendNotificationToUserAsync(
                        userId,
                        new
                        {
                            type = "order_confirmed",
                            orderId = order.OrderId,
                            orderDate = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            claimCode = order.ClaimCode,
                            totalPrice = order.TotalPrice
                        });
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
        // Method to send order confirmation email with invoice
        private async Task SendOrderConfirmationEmailAsync(string email, Order order)
        {
            try
            {
                // Load order details with book information for the email
                var orderDetails = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .ThenInclude(b => b.Authors)
                    .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

                if (orderDetails == null)
                {
                    _logger.LogError("Failed to load order details for email for order {OrderId}", order.OrderId);
                    return;
                }

                // Prepare email body with order details
                string emailBody = $@"
                <h1>Your Order Confirmation</h1>
                <p>Thank you for your order from Book Bagaicha!</p>
                
                <div style='border: 1px solid #ddd; padding: 15px; margin: 15px 0;'>
                    <h2>Order Details</h2>
                    <p><strong>Order ID:</strong> {order.OrderId}</p>
                    <p><strong>Order Date:</strong> {order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss")}</p>
                    <p><strong>Status:</strong> {order.Status}</p>
                    <p><strong>Claim Code:</strong> {order.ClaimCode}</p>
                    
                    <h3>Items:</h3>
                    <table style='width: 100%; border-collapse: collapse;'>
                        <thead>
                            <tr style='background-color: #f2f2f2;'>
                                <th style='padding: 8px; text-align: left; border: 1px solid #ddd;'>Book</th>
                                <th style='padding: 8px; text-align: left; border: 1px solid #ddd;'>Author(s)</th>
                                <th style='padding: 8px; text-align: right; border: 1px solid #ddd;'>Price</th>
                                <th style='padding: 8px; text-align: center; border: 1px solid #ddd;'>Quantity</th>
                                <th style='padding: 8px; text-align: right; border: 1px solid #ddd;'>Total</th>
                            </tr>
                        </thead>
                        <tbody>";

                foreach (var item in orderDetails.OrderItems)
                {
                    var authors = item.Book.Authors.Select(a => $"{a.FirstName} {a.LastName}").ToList();
                    decimal itemTotal = item.PriceAtPurchase * item.Quantity;

                    emailBody += $@"
                            <tr>
                                <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{item.Book.Title}</td>
                                <td style='padding: 8px; text-align: left; border: 1px solid #ddd;'>{string.Join(", ", authors)}</td>
                                <td style='padding: 8px; text-align: right; border: 1px solid #ddd;'>Rs. {item.PriceAtPurchase}/-</td>
                                <td style='padding: 8px; text-align: center; border: 1px solid #ddd;'>{item.Quantity}</td>
                                <td style='padding: 8px; text-align: right; border: 1px solid #ddd;'>Rs. {itemTotal}/-</td>
                            </tr>";
                }

                emailBody += $@"
                        </tbody>
                    </table>
                    
                    <div style='margin-top: 20px; text-align: right;'>
                        <p><strong>Subtotal:</strong> Rs. {(order.TotalPrice + order.AppliedDiscount)}/-</p>";

                if (order.AppliedDiscount > 0)
                {
                    string discountMessage = order.AppliedDiscountIsLoyalty
                        ? "Loyalty Discount (10%):"
                        : "Discount:";

                    emailBody += $@"<p><strong>{discountMessage}</strong> - Rs. {order.AppliedDiscount}/-</p>";

                    if (order.AppliedDiscountIsLoyalty)
                    {
                        emailBody += "<p><small>This discount was applied because you completed 10 orders since your last loyalty discount. Thank you for your continued support!</small></p>";
                    }
                }

                emailBody += $@"
                        <p style='font-size: 1.2em;'><strong>Total:</strong> Rs. {order.TotalPrice}/-</p>
                    </div>
                </div>
                
                <p>Please keep your claim code handy when you come to collect your books from our store.</p>
                <p>If you have any questions about your order, please contact our customer service.</p>
                
                <p>Thank you for shopping with Book Bagaicha!</p>";

                // Send the email
                await _emailService.SendEmailAsync(
                    email,
                    $"Your Book Bagaicha Order Confirmation - {order.ClaimCode}",
                    emailBody,
                    true // isHtml
                );

                // Update the email sent flag
                order.ConfirmationEmailSent = true;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order confirmation email with invoice sent to {Email} for order {OrderId}", email, order.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order confirmation email for order {OrderId}", order.OrderId);
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

        public async Task<OrderDto> GetOrderByClaimCodeAsync(string claimCode)
        {
            try
            {
                _logger.LogInformation("Getting order details for claim code {ClaimCode}", claimCode);

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .ThenInclude(b => b.Authors)
                    .FirstOrDefaultAsync(o => o.ClaimCode == claimCode);

                if (order == null)
                {
                    _logger.LogWarning("Order with claim code {ClaimCode} not found", claimCode);
                    throw new ArgumentException($"Order with claim code {claimCode} not found.");
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
                _logger.LogError(ex, "Error retrieving order details for claim code {ClaimCode}", claimCode);
                throw;
            }
        }

        public async Task<bool> CompleteOrderAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Completing order {OrderId}", orderId);

                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found", orderId);
                    throw new ArgumentException($"Order with ID {orderId} not found.");
                }

                if (order.Status == "Completed")
                {
                    _logger.LogInformation("Order {OrderId} is already completed", orderId);
                    return true;
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    order.Status = "Completed";
                    _context.Orders.Update(order);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Notify customer
                    await _notificationHandler.SendNotificationToUserAsync(
                        order.UserId,
                        new
                        {
                            type = "order_completed",
                            orderId = order.OrderId,
                            orderDate = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            claimCode = order.ClaimCode,
                            totalPrice = order.TotalPrice
                        });

                    // Notify all staff users
                    var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
                    var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                    var allStaffUsers = staffUsers.Concat(adminUsers).Distinct().ToList();

                    foreach (var staffUser in allStaffUsers)
                    {
                        _logger.LogInformation("Sending 'order_completed' notification to staff user {StaffUserId} for order {OrderId}", staffUser.Id, order.OrderId);
                        await _notificationHandler.SendNotificationToUserAsync(
                            (staffUser.Id),
                            new
                            {
                                type = "order_completed",
                                orderId = order.OrderId,
                                orderDate = order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                claimCode = order.ClaimCode,
                                totalPrice = order.TotalPrice
                            });
                    }

                    _logger.LogInformation("Successfully completed order {OrderId}", orderId);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error completing order {OrderId}", orderId);
                    return false;
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while completing order {OrderId}", orderId);
                return false;
            }
        }
    }
}