using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace BookBagaicha.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> CreateBookWithAuthors(BookCreationRequest request)
        {
            var newBook = new Book
            {
                BookId = Guid.NewGuid(),
                Title = request.Title,
                ISBN = request.ISBN,
                Price = request.Price,
                Summary = request.Summary,
                Language = request.Language,
                Format = request.Format,
                PublicationDate = request.PublicationDate.ToUniversalTime(),
                Quantity = request.Quantity,
                OnSale = request.OnSale,
                SalePercntage = request.SalePercntage,
                SaleStartDate = request.SaleStartDate.ToUniversalTime(),
                SaleEndDate = request.SaleEndDate.ToUniversalTime(),
                Category = request.Category,
                Image = request.Image,
                Authors = new List<Author>(),
                Genres = new List<Genre>()
            };


            var authorsToAdd = new List<Author>();

            foreach (var authorName in request.Authors)
            {
                var names = authorName.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (names.Length >= 2)
                {
                    var firstName = names[0];
                    var lastName = string.Join(' ', names.Skip(1));

                    var existingAuthor = await _context.Authors
                        .FirstOrDefaultAsync(a => a.FirstName == firstName && a.LastName == lastName);

                    if (existingAuthor != null)
                    {
                        Console.WriteLine($"Found existing author: {firstName} {lastName}");
                        authorsToAdd.Add(existingAuthor);
                    }
                    else
                    {
                        var newAuthor = new Author { FirstName = firstName, LastName = lastName };
                        Console.WriteLine($"Adding new author: {firstName} {lastName}");
                        _context.Authors.Add(newAuthor);
                        authorsToAdd.Add(newAuthor);
                    }
                }
            }

            // Save authors first to ensure IDs are assigned
            await _context.SaveChangesAsync();

            // Now safely associate authors to the book
            foreach (var author in authorsToAdd)
            {
                newBook.Authors.Add(author);
            }
            var genresToAdd = new List<Genre>();
            foreach (var genreName in request.Genres)
            {
                var existingGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                if (existingGenre != null)
                {
                    Console.WriteLine($"Found existing genre: {genreName}");
                    genresToAdd.Add(existingGenre);
                }
                else
                {
                    var newGenre = new Genre { Name = genreName };
                    Console.WriteLine($"Adding new genre: {genreName}");
                    _context.Genres.Add(newGenre);
                    genresToAdd.Add(newGenre);
                }
            }
            await _context.SaveChangesAsync();  // Save genres before adding to book.
            foreach (var genre in genresToAdd)
            {
                newBook.Genres.Add(genre);
            }

            // Add the book after associating the authors
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            // Reload the book with its authors
            var bookWithAuthors = await _context.Books
                .Include(b => b.Authors) // Ensure Authors are loaded
                .FirstOrDefaultAsync(b => b.BookId == newBook.BookId);

            return bookWithAuthors;
        }


        // Updated GetBookByIdAsync with Include to load authors
        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            return await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres) // Include Genres
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books.Include(b => b.Genres).ToListAsync(); // Include Genres
        }
    }
}
