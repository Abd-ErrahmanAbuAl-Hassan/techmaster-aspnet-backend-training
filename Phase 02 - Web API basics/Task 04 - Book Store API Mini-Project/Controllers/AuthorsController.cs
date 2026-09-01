using BookStoreApi.DTOs;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        /// <summary>
        /// Get all authors
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _authorService.GetAllAuthors();
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get author by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _authorService.GetAuthorById(id);
            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Create a new author
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] CreateAuthorRequest request)
        {
            var result = _authorService.CreateAuthor(request);
            if (result.Success)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.AuthorId }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Delete author by ID
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _authorService.DeleteAuthor(id);
            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }
    }
}