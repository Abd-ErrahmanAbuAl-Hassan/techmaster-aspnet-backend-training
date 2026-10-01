using Microsoft.AspNetCore.Mvc;

namespace Task_06___API_Standards___Refactor_Pack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BadProductsController : ControllerBase
    {
        static List<Product> products = new List<Product>();
        [HttpPost]
        public IActionResult Add(string name, decimal price, int stock)
        {
            if (name == "")
            {
                return Ok("bad name");
            }
            if (price < 0)
            {
                return Ok("bad price");
            }
            var p = new Product();
            p.Id = products.Count + 1;
            p.Name = name;
            p.Price = price;
            p.Stock = stock;
            products.Add(p);
            return Ok(p);
        }
        [HttpGet("all")]
        public IActionResult All()
        {
            return Ok(products);
        }
        [HttpGet("get")]
        public IActionResult Get(int id)
        {
            foreach (var p in products)
            {
                if (p.Id == id)
                {
                    return Ok(p);
                }
            }
            return Ok("not found");
        }
    }
    public class Product
    {
        public int Id; public string Name;
        public decimal Price;
        public int Stock;
    }
}
