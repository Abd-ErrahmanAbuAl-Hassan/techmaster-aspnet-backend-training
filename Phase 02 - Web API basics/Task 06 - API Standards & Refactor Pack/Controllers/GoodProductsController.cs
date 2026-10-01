using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using Task_06___API_Standards___Refactor_Pack.DTOs;
using Task_06___API_Standards___Refactor_Pack.Services;

namespace Task_06___API_Standards___Refactor_Pack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GoodProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public GoodProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpPost]
        public IActionResult Add([FromBody] CreateProductRequest model)
        {
            var result = _productsService.CreateProduct(model);
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    400 => BadRequest(result),
                    404 => NotFound(result),
                    500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                    _ => StatusCode(StatusCodes.Status503ServiceUnavailable, result)
                };
            }
            return CreatedAtAction("GetByID", new {result.Data.Id},result);
        }
        [HttpGet]
        public IActionResult GetAll([FromQuery]int page=1 , [FromQuery]int pageSize = 10)
        {
            var result = _productsService.GetProducts(page, pageSize);
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    400 => BadRequest(result),
                    404 => NotFound(result),
                    500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                    _ => StatusCode(StatusCodes.Status503ServiceUnavailable, result)
                };
            }
            return Ok(result);
        }
        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            var result = _productsService.GetProductById(id);
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    400 => BadRequest(result),
                    404 => NotFound(result),
                    500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                    _ => StatusCode(StatusCodes.Status503ServiceUnavailable, result)
                };
            }
            return Ok(result);
        }
    }
}
