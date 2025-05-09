using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BookBagaicha.Controllers
{

    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly AppDbContext _context;

        public BookController(IBookService bookService, AppDbContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        [HttpPost("api/addBooks")]
        public async Task<IActionResult> AddNewBook([FromForm] BookCreationRequest request)
        {
            if (ModelState.IsValid)
            {
                var newBook = await _bookService.CreateBookWithAuthors(request);
                if (newBook != null)
                {
                    return CreatedAtAction(nameof(GetBookById), new { id = newBook.BookId }, newBook);
                }
                return StatusCode(500, "Failed to create book with authors.");
            }
            return BadRequest(ModelState);
        }

        [HttpGet("api/getBooksByID/{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpGet("api/allBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            var allBooks = await _bookService.GetAllBooksAsync();
            return Ok(allBooks);
        }

        

        [HttpPut("api/updateBooks/{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromForm] UpdateBookRequest request)
        {
            if (id != request.BookId)
            {
                return BadRequest("Book ID in the route does not match the ID in the request body.");
            }

            if (ModelState.IsValid)
            {
                var updatedBook = await _bookService.UpdateBookAsync(request);
                if (updatedBook != null)
                {
                    return Ok(updatedBook); // Or NoContent if you prefer
                }
                return NotFound($"Book with ID '{id}' not found.");
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("api/deleteBooks/{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("api/books/{id}/restock")]
        public async Task<IActionResult> RestockBook(Guid id, [FromBody] int quantityToAdd)
        {
            Console.WriteLine($"RestockBook: id={id}, quantityToAdd={quantityToAdd}");
            if (quantityToAdd <= 0)
            {
                return BadRequest("Quantity to add must be greater than 0.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            book.Quantity += quantityToAdd;
            await _context.SaveChangesAsync();
            return Ok(book);
        }
    }
}
