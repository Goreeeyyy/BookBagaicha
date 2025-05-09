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

        public ReviewService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

        
    }
}
