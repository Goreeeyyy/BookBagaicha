using BookBagaicha.Models.Dto;
using BookBagaicha.Models;

namespace BookBagaicha.IService
{
    public interface IBookService
    {
        Task<Book?> CreateBookWithAuthors(BookCreationRequest request);
        Task<Book?> GetBookByIdAsync(Guid id);
        Task<List<Book>> GetAllBooksAsync();

        Task<Book?> UpdateBookAsync(UpdateBookRequest request);
    }
}
