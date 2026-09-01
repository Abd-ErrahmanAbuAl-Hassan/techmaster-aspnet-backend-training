using BookStoreApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Task_04___Book_Store_API_Mini_Project.Services.Repositories;

namespace BookStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _categoryService.GetAllCategories();
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _categoryService.GetCategoryById(id);
            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] CreateCategoryRequest request)
        {
            var result = _categoryService.CreateCategory(request);
            if (result.Success)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.CategoryId }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Delete category by ID
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _categoryService.DeleteCategory(id);
            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }
    }
}