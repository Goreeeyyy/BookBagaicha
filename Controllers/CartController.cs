using BookBagaicha.IService;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookBagaicha.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        // Get user cart
        [HttpGet("getUserCart")]
        public async Task<IActionResult> GetUserCart()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Getting cart for user {UserId}", userId);
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                return Ok(new { cartId = cart.CartId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user cart");
                return StatusCode(500, "An error occurred while retrieving the cart");
            }
        }

        // Get all cart items
        [HttpGet("getAllCartItems/{cartId}/cartItems")]
        public async Task<IActionResult> GetAllCartItems(Guid cartId)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Getting items for cart {CartId}", cartId);
                var cart = await _cartService.GetCartByIdAsync(cartId);

                // Verify that the cart belongs to the authenticated user
                if (cart.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to access cart {CartId} which belongs to another user", userId, cartId);
                    return Forbid("You are not authorized to access this cart");
                }

                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Cart not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items");
                return StatusCode(500, "An error occurred while retrieving the cart items");
            }
        }

        // Add item to cart
        [HttpPost("addCartItem/{cartId}/cartItems/{bookId}")]
        public async Task<IActionResult> AddCartItem(Guid cartId, Guid bookId, [FromBody] AddToCartRequest request)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                // Verify the cart belongs to the user
                var cart = await _cartService.GetCartByIdAsync(cartId);
                if (cart.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to modify cart {CartId} which belongs to another user", userId, cartId);
                    return Forbid("You are not authorized to modify this cart");
                }

                _logger.LogInformation("Adding book {BookId} to cart {CartId} for user {UserId}", bookId, cartId, userId);
                var cartItem = await _cartService.AddToCartAsync(cartId, bookId, request.Quantity);
                return Ok(cartItem);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error adding to cart: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart: {Message}", ex.Message);
                return StatusCode(500, $"An error occurred while adding to cart: {ex.Message}");
            }
        }

        // Update cart item quantity
        [HttpPut("updateCartItem/{cartId}/cartItems/{bookId}")]
        public async Task<IActionResult> UpdateCartItem(Guid cartId, Guid bookId, [FromBody] UpdateCartItemRequest request)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                // Verify the cart belongs to the user
                var cart = await _cartService.GetCartByIdAsync(cartId);
                if (cart.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to modify cart {CartId} which belongs to another user", userId, cartId);
                    return Forbid("You are not authorized to modify this cart");
                }

                _logger.LogInformation("Updating quantity of book {BookId} in cart {CartId} for user {UserId}", bookId, cartId, userId);
                var cartItem = await _cartService.UpdateCartItemQuantityAsync(cartId, bookId, request.Quantity);
                return Ok(cartItem);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error updating cart item: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item: {Message}", ex.Message);
                return StatusCode(500, $"An error occurred while updating cart item: {ex.Message}");
            }
        }

        // Remove item from cart
        [HttpDelete("deleteCartItem/{cartId}/cartItems/{bookId}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartId, Guid bookId)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                // Verify the cart belongs to the user
                var cart = await _cartService.GetCartByIdAsync(cartId);
                if (cart.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to modify cart {CartId} which belongs to another user", userId, cartId);
                    return Forbid("You are not authorized to modify this cart");
                }

                _logger.LogInformation("Removing book {BookId} from cart {CartId} for user {UserId}", bookId, cartId, userId);
                var success = await _cartService.RemoveFromCartAsync(cartId, bookId);

                if (!success)
                {
                    return NotFound("Cart item not found");
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error removing from cart: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from cart: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while removing from cart");
            }
        }

        // Clear cart
        [HttpDelete("deleteCart")]
        public async Task<IActionResult> DeleteCart([FromQuery] Guid cartID)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                // Verify the cart belongs to the user
                var cart = await _cartService.GetCartByIdAsync(cartID);
                if (cart.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to modify cart {CartId} which belongs to another user", userId, cartID);
                    return Forbid("You are not authorized to modify this cart");
                }

                _logger.LogInformation("Clearing cart {CartId} for user {UserId}", cartID, userId);
                var success = await _cartService.ClearCartAsync(cartID);

                if (!success)
                {
                    return NotFound("Cart not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while clearing cart");
            }
        }
    }
}