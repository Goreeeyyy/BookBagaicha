using BookBagaicha.IService;
using Microsoft.AspNetCore.Mvc;
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
    }
}