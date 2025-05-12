using BookBagaicha.IService;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookBagaicha.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        private readonly IEmailService _emailService;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger, IEmailService emailService)
        {
            _orderService = orderService;
            _logger = logger;
            _emailService = emailService;
        }

        // Get all orders for current user
        [HttpGet("getAllOrder")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Getting all orders for user {UserId}", userId);
                var orders = await _orderService.GetAllOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, "An error occurred while retrieving orders");
            }
        }

        // Get successful order count for the current user
        [HttpGet("getSuccessfulOrderCount")]
        public async Task<IActionResult> GetSuccessfulOrderCount()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Getting successful order count for user {UserId}", userId);
                var count = await _orderService.GetSuccessfulOrderCountAsync(userId);
                return Ok(new { successfulOrderCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting successful order count");
                return StatusCode(500, "An error occurred while retrieving order count");
            }
        }

        // Get order details by ID
        [HttpGet("getOrderDetails/{orderId}/orderitems")]
        public async Task<IActionResult> GetOrderDetails(Guid orderId)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Getting details for order {OrderId}", orderId);
                var orderDetails = await _orderService.GetOrderDetailsAsync(orderId);

                // Verify the order belongs to the current user
                if (orderDetails.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to access order {OrderId} which belongs to another user", userId, orderId);
                    return Forbid("You are not authorized to access this order");
                }

                return Ok(orderDetails);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Order not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order details");
                return StatusCode(500, "An error occurred while retrieving order details");
            }
        }

        // Place a new order
        [HttpPost("placeOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Placing order for user {UserId}", userId);

                // Generate unique claim code
                var claimCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

                var order = await _orderService.PlaceOrderAsync(userId, request, claimCode);

                // Send order confirmation email
                await SendOrderConfirmationEmail(userId, order);
                return Ok(order);

            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized: {Message}", ex.Message);
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order");
                return StatusCode(500, "An error occurred while placing the order");
            }
        }

        // Cancel an order
        [HttpDelete("cancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Cancelling order {OrderId} for user {UserId}", orderId, userId);
                var success = await _orderService.CancelOrderAsync(orderId, userId);

                if (!success)
                {
                    return StatusCode(500, "Failed to cancel the order");
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Order not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized: {Message}", ex.Message);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order");
                return StatusCode(500, "An error occurred while cancelling the order");
            }
        }


        // Send order confirmation email
        private async Task SendOrderConfirmationEmail(long userId, OrderDto order)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("User email not found in claims for user {UserId}", userId);
                    return; // Optionally, retrieve email from a user service
                }

                var emailSubject = "Your BookBagaicha Order Confirmation";
                var emailBody = $@"Dear Customer,

Your order has been successfully placed!
Order ID: {order.OrderId}
Order Code: {order.ClaimCode:N} (Please keep this code for reference)
Total Amount: {order.TotalPrice:C}
Date: {DateTime.UtcNow:dd-MM-yyyy HH:mm:ss UTC}

Thank you for shopping with BookBagaicha!
Best regards,
BookBagaicha Team";

                await _emailService.SendEmailAsync(userEmail, emailSubject, emailBody);
                _logger.LogInformation("Order confirmation email sent to {UserEmail} for order {OrderId}", userEmail, order.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order confirmation email to user {UserId} for order {OrderId}", userId, order.OrderId);
            }
        }
    }
}