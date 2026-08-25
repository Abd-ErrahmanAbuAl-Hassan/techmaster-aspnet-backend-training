using Task_04_Product_Catalog_with_LINQ.DTOs;
using Task_04_Product_Catalog_with_LINQ.Models;

namespace Task_04_Product_Catalog_with_LINQ.Services
{
    internal class ProductQueryService
    {
        private List<Product> _products;
        public ProductQueryService()
        {
            _products = new List<Product>();
            InitializeSeedData();
        }
        private void InitializeSeedData()
        {
            _products = new List<Product>() {

                new Product()
                {
                    ProductId = 1,
                    Name = "Laptop Pro 14",
                    Category = "Electronics",
                    Price = 45000,
                    StockQuantity = 5,
                    CreatedAt = DateTime.Parse("2026-01-10"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 2,
                    Name = "Wireless Mouse",
                    Category = "Electronics",
                    Price = 650,
                    StockQuantity = 50,
                    CreatedAt = DateTime.Parse("2026-02-01"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 3,
                    Name = "Office Chair",
                    Category = "Furniture",
                    Price = 3500,
                    StockQuantity = 10,
                    CreatedAt = DateTime.Parse("2025-12-15"),
                    IsAvailable = true,
                    SupplierName = "HomeSupplier"
                },
                new Product()
                {
                    ProductId = 4,
                    Name = "Standing Desk",
                    Category = "Furniture",
                    Price = 8000,
                    StockQuantity = 3,
                    CreatedAt = DateTime.Parse("2026-03-05"),
                    IsAvailable = true,
                    SupplierName = "HomeSupplier"
                },
                new Product()
                {
                    ProductId = 5,
                    Name = "Notebook Pack",
                    Category = "Stationery",
                    Price = 120,
                    StockQuantity = 100,
                    CreatedAt = DateTime.Parse("2026-01-20"),
                    IsAvailable = true,
                    SupplierName = "PaperSupplier"
                },
                new Product()
                {
                    ProductId = 6,
                    Name = "Pen Set",
                    Category = "Stationery",
                    Price = 75,
                    StockQuantity = 200,
                    CreatedAt = DateTime.Parse("2026-01-25"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 7,
                    Name = "Gaming Keyboard",
                    Category = "Electronics",
                    Price = 2500,
                    StockQuantity = 7,
                    CreatedAt = DateTime.Parse("2026-02-12"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 8,
                    Name = "Monitor 27 inch",
                    Category = "Electronics",
                    Price = 9000,
                    StockQuantity = 4,
                    CreatedAt = DateTime.Parse("2026-02-20"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 9,
                    Name = "LHD Webcam",
                    Category = "Electronics",
                    Price = 1800,
                    StockQuantity = 6,
                    CreatedAt = DateTime.Parse("2026-04-17"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 10,
                    Name = "Desk Lamp",
                    Category = "Furniture",
                    Price = 650,
                    StockQuantity = 0,
                    CreatedAt = DateTime.Parse("2025-11-01"),
                    IsAvailable = false,
                    SupplierName = "HomeSupplier"
                },
                new Product()
                {
                    ProductId = 11,
                    Name = "Backpack",
                    Category = "Accessories",
                    Price = 1200,
                    StockQuantity = 15,
                    CreatedAt = DateTime.Parse("2026-03-10"),
                    IsAvailable = true,
                    SupplierName = "BagSupplier"
                },
                new Product()
                {
                    ProductId = 12,
                    Name = "USB-C Hub",
                    Category = "Electronics",
                    Price = 1250,
                    StockQuantity = 12,
                    CreatedAt = DateTime.Parse("2026-04-01"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 13,
                    Name = "Whiteboard Markers",
                    Category = "Stationery",
                    Price = 95,
                    StockQuantity = 80,
                    CreatedAt = DateTime.Parse("2026-02-15"),
                    IsAvailable = true,
                    SupplierName = "PaperSupplier"
                },
                new Product()
                {
                    ProductId = 14,
                    Name = "Ergonomic Mouse Pad",
                    Category = "Accessories",
                    Price = 350,
                    StockQuantity = 25,
                    CreatedAt = DateTime.Parse("2026-05-01"),
                    IsAvailable = true,
                    SupplierName = "BagSupplier"
                },
                new Product()
                {
                    ProductId = 15,
                    Name = "Meeting Table",
                    Category = "Furniture",
                    Price = 12500,
                    StockQuantity = 2,
                    CreatedAt = DateTime.Parse("2025-10-20"),
                    IsAvailable = true,
                    SupplierName = "HomeSupplier"
                },
                new Product()
                {
                    ProductId = 16,
                    Name = "Printer Paper Box",
                    Category = "Stationery",
                    Price = 450,
                    StockQuantity = 30,
                    CreatedAt = DateTime.Parse("2026-02-28"),
                    IsAvailable = true,
                    SupplierName = "PaperSupplier"
                },
                new Product()
                {
                    ProductId = 17,
                    Name = "Laptop Stand",
                    Category = "Accessories",
                    Price = 950,
                    StockQuantity = 9,
                    CreatedAt = DateTime.Parse("2026-03-30"),
                    IsAvailable = true,
                    SupplierName = "BagSupplier"
                },
                new Product()
                {
                    ProductId = 18,
                    Name = "Network Cable 5m",
                    Category = "Electronics",
                    Price = 150,
                    StockQuantity = 60,
                    CreatedAt = DateTime.Parse("2026-01-05"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 19,
                    Name = "Storage Cabinet",
                    Category = "Furniture",
                    Price = 6000,
                    StockQuantity = 1,
                    CreatedAt = DateTime.Parse("2025-09-10"),
                    IsAvailable = true,
                    SupplierName = "HomeSupplier"
                },
                new Product()
                {
                    ProductId = 20,
                    Name = "Sticky Notes",
                    Category = "Stationery",
                    Price = 60,
                    StockQuantity = 0,
                    CreatedAt = DateTime.Parse("2026-05-10"),
                    IsAvailable = false,
                    SupplierName = "PaperSupplier"
                },
                new Product()
                {
                    ProductId = 21,
                    Name = "Noise Cancelling Headset",
                    Category = "Electronics",
                    Price = 5200,
                    StockQuantity = 4,
                    CreatedAt = DateTime.Parse(" 2026-03-22"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 22,
                    Name = "Desk Organizer",
                    Category = "Accessories",
                    Price = 300,
                    StockQuantity = 40,
                    CreatedAt = DateTime.Parse("2026-06-01"),
                    IsAvailable = true,
                    SupplierName = "BagSupplier"
                },
                new Product()
                {
                    ProductId = 23,
                    Name = "Projector",
                    Category = "Electronics",
                    Price = 22000,
                    StockQuantity = 2,
                    CreatedAt = DateTime.Parse("2026-08-28"),
                    IsAvailable = true,
                    SupplierName = "TechSupplier"
                },
                new Product()
                {
                    ProductId = 24,
                    Name = "Office Sofa",
                    Category = "Furniture",
                    Price = 15500,
                    StockQuantity = 1,
                    CreatedAt = DateTime.Parse("2026-08-18"),
                    IsAvailable = true,
                    SupplierName = "HomeSupplier"
                },
                new Product()
                {
                    ProductId = 25,
                    Name = "Calculator",
                    Category = "Stationery",
                    Price = 250,
                    StockQuantity = 35,
                    CreatedAt = DateTime.Parse("2026-08-12"),
                    IsAvailable = true,
                    SupplierName = "PaperSupplier"
                }

            };
        }

        //LINQ Query 01 - Get All Available Products
        public (bool hasValue, List<Product>? data, string message) GetAvailableProducts()
        {
            var products = _products.Where(p => p.IsAvailable).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");

        }

        //LINQ Query 02 - Filter by Category
        public (bool hasValue, List<Product>? data, string message) FilterByCategory(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return (false, null, "Search term cannot be Empty.");

            var products = _products.Where(p => p.Category.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }

        //LINQ Query 03 - Filter by Price Range
        public (bool hasValue, List<Product>? data, string message) FilterByPriceRange(int min, int max)
        {
            if (min < 0 || max < 0) return (false, null, "Price cannot be negative.");
            if (min > max) return (false, null, "The minimum price value cannot be greater than the maximum value.");

            var products = _products.Where(p => p.Price >= min && p.Price <= max).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }

        //LINQ Query 04 - Search by Product Name
        public (bool hasValue, List<Product>? data, string message) SearchByProductName(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return (false, null, "Search term cannot be Empty.");

            var products = _products.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }

        //LINQ Query 05 - Sort by Price Ascending
        public (bool hasValue, List<Product>? data, string message) SortByPriceAscending()
        {
            var products = _products.OrderBy(p => p.Price).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }

        //LINQ Query 06 - Sort by Price Descending
        public (bool hasValue, List<Product>? data, string message) SortByPriceDescending()
        {
            var products = _products.OrderByDescending(p => p.Price).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }

        //LINQ Query 07 - Group Products by Category
        public (bool hasValue, List<IGrouping<string, Product>>? data, string message) GroupProductsByCategory()
        {
            var products = _products.GroupBy(p => p.Category).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }

        //LINQ Query 08 - Count Products per Category
        public (bool hasValue, List<(string, int)>? data, string message) CountProductsPerCategory()
        {
            var products = _products.GroupBy(p => p.Category).Select(g => (Category: g.Key, Count: g.Count())).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }

        //LINQ Query 09 - Calculate Total Stock Value
        public decimal CalculateTotalStockValue()
        {
            var total = _products.Sum(p => p.Price * p.StockQuantity);

            return total;
        }

        //LINQ Query 10 - Stock Value per Category
        public (bool hasValue, List<(string, decimal)>? data, string message) StockValuePerCategory()
        {
            var products = _products.GroupBy(p => p.Category).Select(g => (Category: g.Key, Total: g.Sum(p => p.Price * p.StockQuantity))).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }

        //LINQ Query 11 - Top 5 Most Expensive Products
        public (bool hasValue, List<Product>? data, string message) TopFiveMostExpensiveProducts()
        {
            var products = _products.OrderByDescending(p => p.Price).Take(5).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }

        //LINQ Query 12 - Low Stock Products
        public (bool hasValue, List<Product>? data, string message) LowStockProducts()
        {
            var products = _products.Where(p => p.StockQuantity <= 5).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }
        //LINQ Query 13 - Out of Stock Products
        public (bool hasValue, List<Product>? data, string message) OutOfStockProducts()
        {
            var products = _products.Where(p => p.StockQuantity == 0 || !p.IsAvailable).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }
        //LINQ Query 14 - Product Summary DTO Projection
        public (bool hasValue, List<ProductSummary>? data, string message) ProductSummaryDTOProjection()
        {
            var products = _products.Select(p => new ProductSummary
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category,
                CreatedAt = p.CreatedAt,
                IsAvailable = p.IsAvailable,
                StockQuantity = p.StockQuantity,
                SupplierName = p.SupplierName,

            }).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }
        //LINQ Query 15 - Supplier Report
        public (bool hasValue, List<SupplierReport>? data, string message) SupplierReport()
        {
            var products = _products.GroupBy(p => p.SupplierName).Select(p => new SupplierReport
            {
                Name = p.Select(p => p.SupplierName).First(),
                Count = p.Count(),
                Stock_Value = p.Sum(s => s.StockQuantity),
                Avg_Price = p.Average(p => p.Price)
            }).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }
        //LINQ Query 16 - Recently Added Products
        public (bool hasValue, List<Product>? data, string message) RecentlyAddedProducts()
        {
            var products = _products.Where(p => p.CreatedAt >= DateTime.Today.AddDays(-15)).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }
        //LINQ Query 17 - Category Statistics
        public (bool hasValue, List<CategoryStats>? data, string message) CategoryStatistics()
        {
            var products = _products.GroupBy(p => p.SupplierName).Select(p => new CategoryStats
            {
                Name = p.Select(p=>p.SupplierName).First(),
                Count = p.Count(),
                Total_Stock_Value = p.Sum(s => s.StockQuantity),
                Average = p.Average(p => p.Price),
                Max_Price = p.Max(p => p.Price),
                Min_Price = p.Min(p => p.Price),

            }).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }
        //LINQ Query 18 - Products Above Average Price
        public (bool hasValue, List<Product>? data, string message) ProductsAboveAveragePrice()
        {
            var avgPrice = _products.Average(p => p.Price);
            var products = _products.Where(p => p.Price >= avgPrice).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }
        //LINQ Query 19 - Search + Filter Combined
        public (bool hasValue, List<Product>? data, string message) SearchAndFilter(string searchTerm, ProductFilter filter)
        {
            var products = _products.ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) 
                                        || p.Category.Contains(filter.Category ?? "", StringComparison.OrdinalIgnoreCase))
                                        .ToList();
            }
            if (filter.Available is not null)
            {
                products = products.Where(p => p.IsAvailable == filter.Available).ToList();
            }

            if (filter.MinPrice is not null)
            {
                products = products.Where(p => p.Price >= filter.MinPrice).OrderBy(p => p.Price).ToList();
            }

            if (filter.MaxPrice is not null)
            {
                products = products.Where(p => p.Price <= filter.MaxPrice).OrderByDescending(p => p.Price).ToList();
            }
            if (!string.IsNullOrWhiteSpace(filter.Category ))
            {
                products = products.Where(p => p.Category == filter.Category).ToList();
            }

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }

            return (true, products, "Successful retrival");
        }
        //LINQ Query 20 - Pagination Simulation
        public (bool hasValue, List<Product>? data, string message) PaginationSimulation(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return (false, null, "Page number and size must be greater than 0.");
            }

            var products = _products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            if (products is null || !products.Any())
            {
                return (false, null, "No products found.");
            }


            return (true, products, "Successful retrival");
        }

    }
}
