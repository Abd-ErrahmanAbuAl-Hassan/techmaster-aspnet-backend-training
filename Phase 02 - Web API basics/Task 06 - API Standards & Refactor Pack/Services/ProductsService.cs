using Task_06___API_Standards___Refactor_Pack.DTOs;
using Task_06___API_Standards___Refactor_Pack.Models;

namespace Task_06___API_Standards___Refactor_Pack.Services
{
    public class ProductsService : IProductsService
    {
        private int _id = 1;
        private static List<Product> _products = new List<Product>();
        public ProductsService()
        {
            SeedInitialProducts();
        }
        private int GetNewId()
        {
            return _id++;
        }

        private void SeedInitialProducts()
        {
            var products = new List<Product>()
            {
                new Product
                {
                    Id = GetNewId(),
                    Name = "Mouse",
                    Price = 50,
                    Stock = 15
                },
                new Product
                {
                    Id = GetNewId(),
                    Name = "Mouse Pad",
                    Price = 10,
                    Stock = 321
                },
                new Product
                {
                    Id = GetNewId(),
                    Name = "Monitor",
                    Price = 1500,
                    Stock = 4
                },
                new Product
                {
                    Id = GetNewId(),
                    Name = "Keyboard",
                    Price = 150,
                    Stock = 50
                },
                new Product
                {
                    Id = GetNewId(),
                    Name = "USB/Type C cable",
                    Price = 30,
                    Stock = 500
                },
                new Product
                {
                    Id = GetNewId(),
                    Name = "RGB keyboard",
                    Price = 310,
                    Stock = 6
                }
            };
            _products.AddRange(products);
        }

        public ApiResponse<ProductResponse> CreateProduct(CreateProductRequest model)
        {
            try
            {
                var errors = new List<string>();
                if (model == null) new ApiResponse<ProductResponse>
                {
                    Success = false,
                    ErrorCode = 400,
                    Errors = new List<string>() { "Creation model is required." },
                    Message = "Validation Error."
                };

                if (model!.Name.Trim().Length < 1) errors.Add("Name is required.");
                if (model!.Price < 1) errors.Add("Price must be a non zero positive number.");
                if (model!.Stock < 0) errors.Add("Price must be a zero or positive number.");

                if (errors.Any()) new ApiResponse<ProductResponse>
                {
                    Success = false,
                    ErrorCode = 400,
                    Errors = errors,
                    Message = "Validation Error."
                };

                var product = new Product
                {
                    Id = GetNewId(),
                    Name = model.Name,
                    Price = model.Price,
                    Stock = model.Stock
                };

                _products.Add(product);

                return new ApiResponse<ProductResponse>
                {
                    Success = true,
                    Message = "Product created successfully.",
                    Data = MapToProductResponse(product)
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<ProductResponse>
                {
                    Success = false,
                    ErrorCode = 500,
                    Errors = new List<string>() { e.Message + "\r\n---" + e.InnerException?.Message },
                    Message = "Internal server error."
                };
            }
        }

        public ApiResponse<ProductResponse> GetProductById(int id)
        {
            try
            {
                if (id < 1) new ApiResponse<ProductResponse>
                {
                    Success = false,
                    ErrorCode = 400,
                    Errors = new List<string>() { "ID must be a positive number." },
                    Message = "Validation Error."
                };

                var product = _products.FirstOrDefault(x => x.Id == id);

                if (product == null) return new ApiResponse<ProductResponse>
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = "Product not found."
                };

                return new ApiResponse<ProductResponse>
                {
                    Success = true,
                    Message = "Product retrieved successfully.",
                    Data = MapToProductResponse(product)
                };

            }
            catch (Exception e)
            {
                return new ApiResponse<ProductResponse>
                {
                    Success = false,
                    ErrorCode = 500,
                    Errors = new List<string>() { e.Message + "\r\n---" + e.InnerException?.Message },
                    Message = "Internal server error."
                };
            }
        }

        public ApiResponse<PagedResult<ProductResponse>> GetProducts(int page = 1, int pageSize = 10)
        {
            try
            {
                if (page < 1 || pageSize < 1) new ApiResponse<PagedResult<ProductResponse>>
                {
                    Success = false,
                    ErrorCode = 400,
                    Errors = new List<string>() { "Page and page size must be a positive number." },
                    Message = "Validation Error."
                };
                var totalCount = _products.Count();
                var totalPages = (totalCount + pageSize - 1) / pageSize;
                var products = _products?.Select(x=> MapToProductResponse(x)).Skip((page-1)* pageSize).Take(pageSize).ToList();
                if (products == null) return new ApiResponse<PagedResult<ProductResponse>>
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = "No products exists."
                };

                return new ApiResponse<PagedResult<ProductResponse>>
                {
                    Success = true,
                    Message = "Products retrieved successfully.",
                    Data = new PagedResult<ProductResponse>
                    {
                        Items = products,
                        TotalCount = totalCount,
                        PageNumber = page,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<PagedResult<ProductResponse>>
                {
                    Success = false,
                    ErrorCode = 500,
                    Errors = new List<string>() { e.Message + "\r\n---" + e.InnerException?.Message },
                    Message = "Internal server error."
                };
            }
        }

        private ProductResponse MapToProductResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };
        }
    }
}
