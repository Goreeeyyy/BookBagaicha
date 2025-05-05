using BookBagaicha.Database;
using BookBagaicha.IService;
using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookBagaicha.Controllers
{
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost("api/addBooks")]
        public async Task<IActionResult> AddNewBook([FromBody] BookCreationRequest request)
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
    }
}