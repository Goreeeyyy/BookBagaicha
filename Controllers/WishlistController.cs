using BookBagaicha.IService;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookBagaicha.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/wishlist")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
        }

        // Get the current user's wishlist
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Getting wishlist for user {UserId}", userId);
                var wishlist = await _wishlistService.GetWishlistByUserIdAsync(userId);
                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlist");
                return StatusCode(500, "An error occurred while retrieving the wishlist");
            }
        }

        // Add a book to the wishlist
        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state: {Errors}",
                        string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Adding book {BookId} to wishlist for user {UserId}", request.BookId, userId);
                var wishlistItem = await _wishlistService.AddToWishlistAsync(userId, request.BookId);
                return Ok(wishlistItem);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Book not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to wishlist: {Message}", ex.Message);
                return StatusCode(500, $"An error occurred while adding to wishlist: {ex.Message}");
            }
        }

        // Remove a book from the wishlist
        [HttpDelete("remove/{wishlistItemId}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid wishlistItemId)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Removing item {WishlistItemId} from wishlist for user {UserId}", wishlistItemId, userId);
                var success = await _wishlistService.RemoveFromWishlistAsync(userId, wishlistItemId);

                if (!success)
                {
                    _logger.LogWarning("Wishlist item {WishlistItemId} not found for user {UserId}", wishlistItemId, userId);
                    return NotFound("Wishlist item not found or does not belong to the current user");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from wishlist: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while removing from wishlist");
            }
        }

        // Check if a book is in the wishlist
        [HttpGet("check/{bookId}")]
        public async Task<IActionResult> IsBookInWishlist(Guid bookId)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
                {
                    _logger.LogWarning("User ID not found in token or invalid");
                    return Unauthorized("Invalid user authentication");
                }

                _logger.LogInformation("Checking if book {BookId} is in wishlist for user {UserId}", bookId, userId);
                var isInWishlist = await _wishlistService.IsBookInWishlistAsync(userId, bookId);
                return Ok(new { isInWishlist });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if book is in wishlist: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while checking wishlist");
            }
        }
    }
}