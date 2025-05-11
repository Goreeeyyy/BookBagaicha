using BookBagaicha.IService;
using BookBagaicha.Models.Dto;
using BookBagaicha.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookBagaicha.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
           
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetBookReviews(Guid bookId)
        {
            var reviews = await _reviewService.GetReviewsByBookId(bookId);
            return Ok(reviews);
        }

        [HttpPost("addReview")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                var reviewDto = await _reviewService.AddReviewAsync(userId, request.BookId, request.Ratings, request.Comments);
                return CreatedAtAction(nameof(GetBookReviews), new { bookId = request.BookId }, reviewDto);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the error in the controller as well for extra visibility
                System.Console.WriteLine($"Error in AddReview Controller: {ex}");
                return StatusCode(500, new { message = "Failed to add review." });
            }
        }

    }
}