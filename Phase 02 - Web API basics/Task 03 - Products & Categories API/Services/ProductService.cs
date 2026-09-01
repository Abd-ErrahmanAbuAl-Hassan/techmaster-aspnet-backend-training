using Task_03_Products_Categories_API.DTOs;
using Task_03_Products_Categories_API.Entities;
using Task_03_Products_Categories_API.Utilities;

namespace Task_03_Products_Categories_API.Services
{
    public class ProductService
    {
        private static List<Product> _products = new List<Product>();
        private readonly CategoryService _categoryService;

        public ProductService(CategoryService categoryService)
        {
            _categoryService = categoryService;
            InitializeDataSeed();
        }
        public Result<ProductResponse> Create(CreateProductRequest model)
        {
            if (model == null) return new Result<ProductResponse>
            {
                Success = false,
                ErrorCode = 400,
                Message = "Creation model is required",
                Errors = new List<string> { "model is null" }
            };

            var hasErrors = ValidateProduct(model, out var category, out var errors);

            if (hasErrors)
            {
                return new Result<ProductResponse>
                {
                    Success = false,
                    ErrorCode = 400,
                    Message = $"Validation errors.",
                    Errors = errors
                };
            }

            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                IsAvailable = model.StockQuantity > 0 ? true : false,
                SupplierName = model.SupplierName,
                Category = category

            };
            _products.Add(product);
            var productResponse = MapToProductResponse(product);
            return new Result<ProductResponse>
            {
                Success = true,
                Message = $"Product created successfully.",
                Data = productResponse
            };
        }
        public Result<ProductResponse> Update(Guid id, UpdateProductRequest model)
        {
            if (model == null) return new Result<ProductResponse>
            {
                Success = false,
                ErrorCode = 400,
                Message = "Creation model is required",
                Errors = new List<string> { "model is null" }
            };

            var result = ValidateProduct(model, out var category, out var errors);

            if (result)
            {
                return new Result<ProductResponse>
                {
                    Success = false,
                    ErrorCode = 400,
                    Message = $"Validation errors.",
                    Errors = errors
                };
            }

            var product = _products.FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return new Result<ProductResponse>
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = $"Product not found.",
                    Errors = new List<string> { "Invalid product ID." }
                };
            }

            var index = _products.IndexOf(product);

            if (model.Name != null) product.Name = model.Name;
            if (model.SupplierName != null) product.SupplierName = model.SupplierName;
            if (model.Price != null) product.Price = (decimal)model.Price;
            if (model.StockQuantity != null)
            {
                product.StockQuantity = (int)model.StockQuantity;
                product.IsAvailable = model.StockQuantity > 0 ? true : false;
            }
            if (category != null) product.Category = category;


            _products[index] = product;
            var productResponse = MapToProductResponse(product);
            return new Result<ProductResponse>
            {
                Success = true,
                Message = $"Product updated successfully.",
                Data = productResponse
            };

        }
        public Result<Product> Delete(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return new Result<Product>
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = $"Product not found.",
                    Errors = new List<string> { "Invalid product ID." }
                };
            }
            _products.Remove(product);

            return new Result<Product> { Success = true, Message = "Product deleted successfully." };
        }
        public Result<List<ProductResponse>> GetAllProducts(PFilter filter)
        {
            if (!_products.Any())
                return new Result<List<ProductResponse>>
                {
                    Success = false,
                    Message = "No Products Exists, create the first one.",
                    ErrorCode = 404
                };

            var errors = new List<string>();
            var products = _products.Select(p => MapToProductResponse(p)).ToList();


            if (filter.SearchTerm is not null)
            {
                products = products.Where(s => s.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)
                                            || s.SupplierName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (filter.IsAvilable is not null)
            {
                products = products.Where(s => s.IsAvailable == filter.IsAvilable).ToList();
            }

            if (filter.MaxPrice != null && filter.MinPrice != null)
            {
                if (filter.MinPrice > filter.MaxPrice) errors.Add("Minimum price can not be lower than maximum price.");
                else
                {
                    products = products.Where(p => p.Price >= filter.MinPrice && p.Price <= filter.MaxPrice).ToList();
                }

            }

            if (filter.Category is not null)
            {
                products = products.Where(p => p.Category == filter.Category).ToList();
            }
            if (filter.LowStockThreshold is not null)
            {
                products = products.Where(p => p.StockQuantity > 0 && p.StockQuantity <= filter.LowStockThreshold).ToList();
            }
            var count = products.Count;
            if (filter.PageSize is not null && filter.Page is not null)
            {
                if (filter.Page < 1) errors.Add("Page number must be greater than 1.");
                if (filter.PageSize < 1) errors.Add("Page size must be greater than 1.");
                if (filter.PageSize > 50) errors.Add("Page size must be less than 50.");
                if (errors.Any()) return new Result<List<ProductResponse>>
                {
                    Success = false,
                    Message = "Pagination parameters are not valid.",
                    Errors = errors,
                    ErrorCode = 400
                };

                products = products.Skip((int)((filter.Page - 1) * filter.PageSize))
                                   .Take((int)filter.PageSize).ToList();

            }

            if (!products.Any())
                return new Result<List<ProductResponse>>
                {
                    Success = false,
                    Message = "No Categories found.",
                    ErrorCode = 404
                };

            return new Result<List<ProductResponse>>
            {
                Success = true,
                Message = $"Successfully retrieve ({count}) products.",
                Data = products
            };
        }

        public Result<ProductResponse> GetProductById(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null) return new Result<ProductResponse>()
            {
                Success = false,
                Message = "Product not found.",
                ErrorCode = 404,
                Errors = new List<string> { $"Invalid product ID." }
            };

            var productResponse = MapToProductResponse(product);

            return new Result<ProductResponse>()
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = productResponse
            };


        }

        public Result<StockReportDto> GetStockReport(int lowStockThreshold = 10)
        {
            if (!_products.Any())
            {
                return new Result<StockReportDto>
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = "No products exist in the system.",
                    Errors = { "Product list is empty." }
                };
            }

            try
            {
                var report = new StockReportDto();

                // Calculate total stock value
                report.TotalStockValue = _products.Sum(p => p.Price * p.StockQuantity);

                // Stock value per category
                report.StockValuePerCategory = _products
                    .GroupBy(p => p.Category.Name)
                    .Select(g => new CategoryStockDto
                    {
                        CategoryName = g.Key,
                        TotalValue = g.Sum(p => p.Price * p.StockQuantity),
                        ProductCount = g.Count()
                    })
                    .OrderByDescending(c => c.TotalValue)
                    .ToList();

                // Low stock products
                report.LowStockProducts = _products
                    .Where(p => p.StockQuantity > 0 && p.StockQuantity <= lowStockThreshold)
                    .Select(p => new LowStockProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        CategoryName = p.Category.Name,
                        CurrentStock = p.StockQuantity,
                        LowStockThreshold = lowStockThreshold,
                        Price = p.Price,
                        TotalValue = p.Price * p.StockQuantity
                    })
                    .OrderBy(p => p.CurrentStock)
                    .ToList();

                // Out of stock products
                report.OutOfStockProducts = _products
                    .Where(p => p.StockQuantity == 0)
                    .Select(p => new StockValueDto
                    {
                        ProductName = p.Name,
                        Quantity = p.StockQuantity,
                        UnitPrice = p.Price,
                        TotalValue = 0
                    })
                    .ToList();

                // Products count by category
                report.ProductCountByCategory = _products
                    .GroupBy(p => p.Category.Name)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Summary counts
                report.TotalProductCount = _products.Count();
                report.TotalOutOfStockCount = report.OutOfStockProducts.Count();
                report.TotalLowStockCount = report.LowStockProducts.Count();

                return new Result<StockReportDto>
                {
                    Success = true,
                    Message = "Stock report generated successfully.",
                    Data = report
                };
            }
            catch (Exception ex)
            {
                return new Result<StockReportDto>
                {
                    Success = false,
                    ErrorCode = 500,
                    Message = "An error occurred while generating the stock report.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        private void InitializeDataSeed()
        {
            var products = new List<Product>()
            {
                new Product
                {
                    Name ="Laptop",
                    Price = 45000,
                    StockQuantity= 5,
                    IsAvailable= true,
                    SupplierName="TechSupplier",
                    Category = _categoryService.GetCategoryByName("Electronics")
                },
                new Product
                {
                    Name ="Mouse",
                    Price = 50,
                    StockQuantity= 354,
                    IsAvailable= true,
                    SupplierName="TechSupplier",
                    Category = _categoryService.GetCategoryByName("Electronics")
                },
                new Product
                {
                    Name ="Keyboard",
                    Price = 3500,
                    StockQuantity= 621,
                    IsAvailable= true,
                    SupplierName="TechSupplier",
                    Category = _categoryService.GetCategoryByName("Electronics")
                },
                new Product
                {
                    Name ="Monitor",
                    Price = 9000,
                    StockQuantity= 16,
                    IsAvailable= true,
                    SupplierName="TechSupplier",
                    Category = _categoryService.GetCategoryByName("Electronics")
                },
                new Product
                {
                    Name ="USB-C Hub",
                    Price = 500,
                    StockQuantity= 20,
                    IsAvailable= true,
                    SupplierName="TechSupplier",
                    Category = _categoryService.GetCategoryByName("Electronics")
                },
                new Product
                {
                    Name ="Office Chair",
                    Price =3500 ,
                    StockQuantity= 10,
                    IsAvailable= true,
                    SupplierName="HomeSupplier",
                    Category = _categoryService.GetCategoryByName("Furniture")
                },
                new Product
                {
                    Name ="Desk",
                    Price = 8000,
                    StockQuantity= 3,
                    IsAvailable= true,
                    SupplierName="HomeSupplier",
                    Category = _categoryService.GetCategoryByName("Furniture")
                },
                new Product
                {
                    Name ="Desk Lamp",
                    Price = 25000,
                    StockQuantity= 0,
                    IsAvailable= false,
                    SupplierName="HomeSupplier",
                    Category = _categoryService.GetCategoryByName("Furniture")
                },
                new Product
                {
                    Name ="Notebook",
                    Price = 5,
                    StockQuantity=156 ,
                    IsAvailable= true,
                    SupplierName="PaperSupplier",
                    Category = _categoryService.GetCategoryByName("Stationery")
                },
                new Product
                {
                    Name ="Pen Set",
                    Price = 12,
                    StockQuantity= 0,
                    IsAvailable= false,
                    SupplierName="PaperSupplier",
                    Category = _categoryService.GetCategoryByName("Stationery")
                },
                new Product
                {
                    Name ="Marker",
                    Price = 20,
                    StockQuantity= 80,
                    IsAvailable= true,
                    SupplierName="PaperSupplier",
                    Category = _categoryService.GetCategoryByName("Stationery")
                },
                new Product
                {
                    Name ="Paper Pack",
                    Price = 15 ,
                    StockQuantity= 0 ,
                    IsAvailable= false,
                    SupplierName="PaperSupplier",
                    Category = _categoryService.GetCategoryByName("Stationery")
                },
                new Product
                {
                    Name ="Backpack",
                    Price = 250 ,
                    StockQuantity=  30,
                    IsAvailable= true,
                    SupplierName="BagSupplier",
                    Category = _categoryService.GetCategoryByName("Accessories")
                },
                new Product
                {
                    Name ="Mouse Pad",
                    Price = 30 ,
                    StockQuantity=  12,
                    IsAvailable= true,
                    SupplierName="BagSupplier",
                    Category = _categoryService.GetCategoryByName("Accessories")
                },
                new Product
                {
                    Name ="Laptop Sleeve",
                    Price = 50 ,
                    StockQuantity=  8,
                    IsAvailable= true,
                    SupplierName="BagSupplier",
                    Category = _categoryService.GetCategoryByName("Accessories")
                },
            };
            _products.AddRange(products);
        }

        private bool ValidateProduct(object obj, out Category category, out List<string> errors)
        {
            errors = new List<string>();
            category = null;

            if (obj is CreateProductRequest)
            {
                var model = (CreateProductRequest)obj;
                category = _categoryService.GetCategoryById(model.CategoryId).Data;

                if (category is null) errors.Add($"Category with Id: '{model.CategoryId}' is not found.");
                if (model.Price <= 0) errors.Add("Price must be greater than 0.");
                if (model.StockQuantity < 0) errors.Add("Stock quantity must be not negative.");
            }
            else if (obj is UpdateProductRequest)
            {
                var model = (UpdateProductRequest)obj;
                
                if (model.CategoryId is not null)
                {
                    category = _categoryService.GetCategoryById((Guid)model.CategoryId).Data;
                    if (category is null) errors.Add($"Category with Id: '{model.CategoryId}' is not found.");
                }

                if (model.Price is not null && model.Price <= 0) errors.Add("Price must be greater than 0.");
                if (model.StockQuantity is not null && model.StockQuantity < 0) errors.Add("Stock quantity must be not negative.");
            }
            else
            {
                errors.Add("Invalid model type for validation.");
                return true;
            }

            return errors.Any();
        }

        private ProductResponse MapToProductResponse(Product product) => new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt,
            SupplierName = product.SupplierName,
            Category = product.Category.Name
        };

    }
}
