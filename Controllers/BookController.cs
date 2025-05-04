using BookBagaicha.Database;
using BookBagaicha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookBagaicha.Controllers
{
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("api/books/add")]
        public async Task<IActionResult> AddNewBook([FromBody] Book newBook)
        {
            if (ModelState.IsValid)
            {
                newBook.BookId = Guid.NewGuid(); // Generate a new unique ID for the book
                _context.Books.Add(newBook);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetBookById), new { id = newBook.BookId }, newBook);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("api/books/{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        [HttpGet("api/books")]
        public async Task<IActionResult> GetAllBooks()
        {
            var allBooks = await _context.Books.ToListAsync();
            return Ok(allBooks);
        }
    }
}