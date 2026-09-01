using BookStoreApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Task_04___Book_Store_API_Mini_Project.Services.Repositories;

namespace BookStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Get all books with search, filtering, and pagination
        /// </summary>
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchQuery = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? authorId = null,
            [FromQuery] bool? isAvailable = null)
        {
            var result = _bookService.GetAllBooks(pageNumber, pageSize, searchQuery, categoryId, authorId, isAvailable);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get book by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _bookService.GetBookById(id);
            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Create a new book
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] CreateBookRequest request)
        {
            var result = _bookService.CreateBook(request);
            if (result.Success)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.BookId }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Update book by ID
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateBookRequest request)
        {
            var result = _bookService.UpdateBook(id, request);
            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Delete book by ID
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _bookService.DeleteBook(id);
            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Get books summary with statistics
        /// </summary>
        [HttpGet("reports/summary")]
        public IActionResult GetSummary()
        {
            var result = _bookService.GetBooksSummary();
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}