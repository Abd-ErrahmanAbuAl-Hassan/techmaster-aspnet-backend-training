using Microsoft.AspNetCore.Mvc;
using Task_03_Products_Categories_API.DTOs;
using Task_03_Products_Categories_API.Entities;
using Task_03_Products_Categories_API.Services;
using Task_03_Products_Categories_API.Utilities;

namespace Task_02_Student_Management_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }


        [HttpPost]
        public IActionResult Create([FromBody] CreateProductRequest model)
        {
            var result = _productService.Create(model);
            
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }


        [HttpGet]
        public IActionResult GetAll([FromQuery] PFilter filter)
        {
            var result = _productService.GetAllProducts(filter);

            if (!result.Success && result.ErrorCode == 400)
                return BadRequest(result);

            if (!result.Success && result.ErrorCode == 404)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var result = _productService.GetProductById(id);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }


        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] UpdateProductRequest model)
        {
            var result = _productService.Update(id, model);
            
            if (!result.Success && result.ErrorCode == 400)
                return BadRequest(result);

            if (!result.Success && result.ErrorCode == 404)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _productService.Delete(id);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("reports/stock")]
        public IActionResult GetStockReport([FromQuery] int lowStockThreshold = 10)
        {
            var result = _productService.GetStockReport(lowStockThreshold);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

    }
}
