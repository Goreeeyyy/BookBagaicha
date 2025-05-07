using BookBagaicha.IService;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookBagaicha.Controllers
{
    [ApiController]
    [Authorize] // Require authentication for all wishlist operations
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // Get the current user's wishlist
        [HttpGet("api/wishlist")]
        public async Task<IActionResult> GetWishlist()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var wishlist = await _wishlistService.GetWishlistByUserIdAsync(userId);
            return Ok(wishlist);
        }

        // Add a book to the wishlist
        [HttpPost("api/wishlist/add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                var wishlistItem = await _wishlistService.AddToWishlistAsync(userId, request.BookId);
                return Ok(wishlistItem);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Remove a book from the wishlist
        [HttpDelete("api/wishlist/remove/{wishlistItemId}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid wishlistItemId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var success = await _wishlistService.RemoveFromWishlistAsync(userId, wishlistItemId);
            if (!success)
            {
                return NotFound("Wishlist item not found or does not belong to the current user.");
            }

            return NoContent();
        }

        // Check if a book is in the wishlist
        [HttpGet("api/wishlist/check/{bookId}")]
        public async Task<IActionResult> IsBookInWishlist(Guid bookId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var isInWishlist = await _wishlistService.IsBookInWishlistAsync(userId, bookId);
            return Ok(new { isInWishlist });
        }
    }
}
