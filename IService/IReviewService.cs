using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BookBagaicha.Models.Dto.ReviewDTO;

namespace BookBagaicha.IService
{
    public interface IReviewService
    {
        Task<List<ReviewDTO>> GetReviewsByBookId(Guid bookId);
        Task<ReviewDTO> AddReviewAsync(long userId, Guid bookId, int ratings, string? comments);

    }
}