using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace BookBagaicha.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderService> _logger;


        public ReviewService(AppDbContext dbContext, ILogger<OrderService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<ReviewDTO>> GetReviewsByBookId(Guid bookId)
        {
            return await _dbContext.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.User)
                .Select(r => new ReviewDTO
                {
                    ReviewId = r.ReviewId,
                    BookId = r.BookId,
                    UserId = r.UserId,
                    Ratings = r.Ratings,
                    Comments = r.Comments,
                    ReviewDate = r.ReviewDate,
                    UserDisplayName = r.User.UserName // Or User.FirstName + " " + User.LastName, etc.
                })
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }
        public async Task<ReviewDTO> AddReviewAsync(long userId, Guid bookId, int ratings, string? comments)
        {
            try
            {
                var book = await _dbContext.Books.FindAsync(bookId);
                var user = await _dbContext.Users.FindAsync(userId);

                if (book == null)
                {
                    _logger.LogWarning("Book with ID {BookId} not found while adding review.", bookId);
                    throw new ArgumentException($"Book with ID {bookId} not found.");
                }

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found while adding review.", userId);
                    throw new ArgumentException($"User with ID {userId} not found.");
                }

                var newReview = new Review
                {
                    BookId = bookId,
                    UserId = userId,
                    Ratings = ratings,
                    Comments = comments,
                    ReviewDate = DateTime.UtcNow
                };

                _dbContext.Reviews.Add(newReview);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Review added for book {BookId} by user {UserId}.", bookId, userId);

                // Fetch the newly added review with user details for the DTO
                var addedReview = await _dbContext.Reviews
                    .Where(r => r.ReviewId == newReview.ReviewId)
                    .Include(r => r.User)
                    .Select(r => new ReviewDTO
                    {
                        ReviewId = r.ReviewId,
                        BookId = r.BookId,
                        UserId = r.UserId,
                        Ratings = r.Ratings,
                        Comments = r.Comments,
                        ReviewDate = r.ReviewDate,
                        UserDisplayName = r.User.UserName
                    })
                    .FirstOrDefaultAsync();

                return addedReview;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review for book {BookId} by user {UserId}.", bookId, userId);
                throw;
            }
        }


    }
}
