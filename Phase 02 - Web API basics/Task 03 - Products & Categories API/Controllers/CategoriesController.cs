using Microsoft.AspNetCore.Mvc;
using Task_03_Products_Categories_API.DTOs;
using Task_03_Products_Categories_API.Entities;
using Task_03_Products_Categories_API.Services;
using Task_03_Products_Categories_API.Utilities;

namespace Task_02_Student_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly CatalogService _catalogService;

        public CategoriesController(CategoryService categoryService, CatalogService catalogService)
        {
            _categoryService = categoryService;
            _catalogService = catalogService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCategoryRequest model)
        {
            var result = _categoryService.Create(model);

            if (!result.Success && result.ErrorCode == 400)
                return BadRequest(result);

            if (!result.Success && result.ErrorCode == 409)
                return Conflict(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] CFilter filter)
        {
            var result = _catalogService.GetCategoriesWithProducts(filter);

            if (!result.Success && result.ErrorCode == 400)
                return BadRequest(result);

            if (!result.Success && result.ErrorCode == 404)
                return NotFound(result);

            return Ok(result);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var result = _catalogService.GetCategoryWithProducts(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] UpdateCategoryRequest model)
        {
            var result = _categoryService.Update(id, model);

            if (!result.Success && result.ErrorCode == 400)
                return BadRequest(result);

            if (!result.Success && result.ErrorCode == 404)
                return NotFound(result);
            if (!result.Success && result.ErrorCode == 409)
                return Conflict(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _categoryService.Delete(id);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
