using Task_04_Product_Catalog_with_LINQ.DTOs;
using Task_04_Product_Catalog_with_LINQ.Models;
using Task_04_Product_Catalog_with_LINQ.Services;

namespace Task_04_Product_Catalog_with_LINQ.UI
{
    internal class ConsoleMenu
    {
        private readonly ProductQueryService _productService;
        private bool _isRunning;

        public ConsoleMenu(ProductQueryService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _isRunning = true;
        }

        public void Run()
        {
            DisplayWelcome();

            while (_isRunning)
            {
                DisplayMainMenu();
                HandleMenuChoice();
            }

            DisplayGoodbye();
        }

        private void DisplayWelcome()
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║   PRODUCT CATALOG QUERY SERVICE            ║");
            Console.WriteLine("║   Advanced LINQ Query Demonstrations       ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine("                   MAIN MENU                    ");
            Console.WriteLine("═══════════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("  ► BASIC QUERIES");
            Console.WriteLine("    [1]  View All Available Products");
            Console.WriteLine("    [2]  Search Product by Name");
            Console.WriteLine("    [3]  Filter by Category");
            Console.WriteLine("    [4]  Filter by Price Range");
            Console.WriteLine();
            Console.WriteLine("  ► SORTING & ANALYSIS");
            Console.WriteLine("    [5]  Sort by Price (Ascending)");
            Console.WriteLine("    [6]  Sort by Price (Descending)");
            Console.WriteLine("    [7]  Top 5 Most Expensive Products");
            Console.WriteLine("    [8]  Low Stock Products (≤5 units)");
            Console.WriteLine("    [9]  Out of Stock Products");
            Console.WriteLine();
            Console.WriteLine("  ► GROUPING & REPORTS");
            Console.WriteLine("    [10] Group Products by Category");
            Console.WriteLine("    [11] Count Products per Category");
            Console.WriteLine("    [12] Stock Value per Category");
            Console.WriteLine("    [13] Category Statistics");
            Console.WriteLine("    [14] Supplier Report");
            Console.WriteLine();
            Console.WriteLine("  ► ADVANCED");
            Console.WriteLine("    [15] Total Stock Value (All Products)");
            Console.WriteLine("    [16] Recently Added Products");
            Console.WriteLine("    [17] Product Summary (Projection)");
            Console.WriteLine("    [18] Products Above Average Price");
            Console.WriteLine("    [19] Advanced Search & Filter");
            Console.WriteLine("    [20] Pagination (Page 1)");
            Console.WriteLine();
            Console.WriteLine("  ► EXIT");
            Console.WriteLine("    [0]  Exit Application");
            Console.WriteLine();
            Console.Write("  Choose an option (0-20): ");
        }

        private void HandleMenuChoice()
        {
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                DisplayError("Invalid input. Please enter a number between 0 and 20.");
                PauseForUser();
                return;
            }

            switch (choice)
            {
                case 1:
                    GetAvailableProducts();
                    break;
                case 2:
                    SearchByProductName();
                    break;
                case 3:
                    FilterByCategory();
                    break;
                case 4:
                    FilterByPriceRange();
                    break;
                case 5:
                    SortByPriceAscending();
                    break;
                case 6:
                    SortByPriceDescending();
                    break;
                case 7:
                    TopFiveMostExpensiveProducts();
                    break;
                case 8:
                    LowStockProducts();
                    break;
                case 9:
                    OutOfStockProducts();
                    break;
                case 10:
                    GroupProductsByCategory();
                    break;
                case 11:
                    CountProductsPerCategory();
                    break;
                case 12:
                    StockValuePerCategory();
                    break;
                case 13:
                    CategoryStatistics();
                    break;
                case 14:
                    SupplierReport();
                    break;
                case 15:
                    CalculateTotalStockValue();
                    break;
                case 16:
                    RecentlyAddedProducts();
                    break;
                case 17:
                    ProductSummaryDTOProjection();
                    break;
                case 18:
                    ProductsAboveAveragePrice();
                    break;
                case 19:
                    SearchAndFilterAdvanced();
                    break;
                case 20:
                    PaginationSimulation();
                    break;
                case 0:
                    _isRunning = false;
                    break;
                default:
                    DisplayError("Invalid option. Please choose between 0 and 20.");
                    PauseForUser();
                    break;
            }
        }

        private void GetAvailableProducts()
        {
            ClearScreen();
            DisplayHeader("Available Products");

            var result = _productService.GetAvailableProducts();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  Found {result.data.Count} available product(s):\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void SearchByProductName()
        {
            ClearScreen();
            DisplayHeader("Search Product by Name");

            Console.Write("  Enter product name (or part of name): ");
            string searchTerm = Console.ReadLine() ?? string.Empty;

            var result = _productService.SearchByProductName(searchTerm);

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  Found {result.data.Count} product(s):\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void FilterByCategory()
        {
            ClearScreen();
            DisplayHeader("Filter by Category");

            Console.Write("  Enter category name: ");
            string category = Console.ReadLine() ?? string.Empty;

            var result = _productService.FilterByCategory(category);

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  Found {result.data.Count} product(s) in '{category}':\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void FilterByPriceRange()
        {
            ClearScreen();
            DisplayHeader("Filter by Price Range");

            if (!GetPriceInput(out int min, out int max))
            {
                PauseForUser();
                return;
            }

            var result = _productService.FilterByPriceRange(min, max);

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  Found {result.data.Count} product(s) in range {min:N0} - {max:N0}:\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void SortByPriceAscending()
        {
            ClearScreen();
            DisplayHeader("Products Sorted by Price (Ascending)");

            var result = _productService.SortByPriceAscending();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void SortByPriceDescending()
        {
            ClearScreen();
            DisplayHeader("Products Sorted by Price (Descending)");

            var result = _productService.SortByPriceDescending();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void TopFiveMostExpensiveProducts()
        {
            ClearScreen();
            DisplayHeader("Top 5 Most Expensive Products");

            var result = _productService.TopFiveMostExpensiveProducts();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void LowStockProducts()
        {
            ClearScreen();
            DisplayHeader("Low Stock Products (5 units or less)");

            var result = _productService.LowStockProducts();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  {result.data.Count} product(s) with low stock:\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void OutOfStockProducts()
        {
            ClearScreen();
            DisplayHeader("Out of Stock Products");

            var result = _productService.OutOfStockProducts();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  {result.data.Count} product(s) out of stock:\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void GroupProductsByCategory()
        {
            ClearScreen();
            DisplayHeader("Products Grouped by Category");

            var result = _productService.GroupProductsByCategory();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine();
                foreach (var group in result.data)
                {
                    Console.WriteLine($"  {group.Key} ({group.Count()} products)");
                    Console.WriteLine("  " + new string('─', 97));
                    DisplayProductsTable(group.ToList());
                    Console.WriteLine();
                }
            }

            PauseForUser();
        }

        private void CountProductsPerCategory()
        {
            ClearScreen();
            DisplayHeader("Product Count per Category");

            var result = _productService.CountProductsPerCategory();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("  ┌─────────────────────────┬───────────┐");
                Console.WriteLine("  │ Category                │  Count    │");
                Console.WriteLine("  ├─────────────────────────┼───────────┤");

                foreach (var item in result.data)
                {
                    Console.WriteLine($"  │ {item.Item1,-23} │ {item.Item2,4:N0}      │");
                }

                Console.WriteLine("  └─────────────────────────┴───────────┘");
                Console.WriteLine();
            }

            PauseForUser();
        }

        private void StockValuePerCategory()
        {
            ClearScreen();
            DisplayHeader("Stock Value per Category");

            var result = _productService.StockValuePerCategory();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("  ┌─────────────────────────┬──────────────────┐");
                Console.WriteLine("  │ Category                │   Stock Value    │");
                Console.WriteLine("  ├─────────────────────────┼──────────────────┤");

                foreach (var item in result.data)
                {
                    Console.WriteLine($"  │ {item.Item1,-23} │ {item.Item2,-13:N0}    │");
                }

                Console.WriteLine("  └─────────────────────────┴──────────────────┘");
                Console.WriteLine();
            }

            PauseForUser();
        }

        private void CategoryStatistics()
        {
            ClearScreen();
            DisplayHeader("Category Statistics");

            var result = _productService.CategoryStatistics();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine();
                foreach (var stat in result.data)
                {
                    Console.WriteLine($"  - Statistics for {stat.Name}");
                    Console.WriteLine($"     Total Products:      {stat.Count}");
                    Console.WriteLine($"     Total Stock Value:   {stat.Total_Stock_Value} units");
                    Console.WriteLine($"     Average Price:       {stat.Average:N2}$");
                    Console.WriteLine($"     Highest Price:       {stat.Max_Price:N0}$");
                    Console.WriteLine($"     Lowest Price:        {stat.Min_Price:N0}$");
                    Console.WriteLine();
                }
            }

            PauseForUser();
        }

        private void SupplierReport()
        {
            ClearScreen();
            DisplayHeader("Supplier Report");

            var result = _productService.SupplierReport();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("  ┌──────────────────┬───────────┬─────────────┬──────────────┐");
                Console.WriteLine("  │ Supplier         │  Count    │ Stock Units │ Avg Price    │");
                Console.WriteLine("  ├──────────────────┼───────────┼─────────────┼──────────────┤");

                foreach (var item in result.data)
                {
                    Console.WriteLine($"  │ {item.Name,-16} │ {item.Count,-7:N0}   │ {item.Stock_Value,-9:N0}   │ {item.Avg_Price,-8:F2}$    │");
                }

                Console.WriteLine("  └──────────────────┴───────────┴─────────────┴──────────────┘");
                Console.WriteLine();
            }

            PauseForUser();
        }

        private void CalculateTotalStockValue()
        {
            ClearScreen();
            DisplayHeader("Total Stock Value (All Products)");

            decimal totalValue = _productService.CalculateTotalStockValue();

            Console.WriteLine();
            Console.WriteLine($" - Total Stock Value: {totalValue:N0}$");
            Console.WriteLine();

            PauseForUser();
        }

        private void RecentlyAddedProducts()
        {
            ClearScreen();
            DisplayHeader("Recently Added Products (Last 15 days)");

            var result = _productService.RecentlyAddedProducts();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  Found {result.data.Count} product(s) added recently:\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void ProductSummaryDTOProjection()
        {
            ClearScreen();
            DisplayHeader("Product Summary (Projection)");

            var result = _productService.ProductSummaryDTOProjection();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                DisplayProductSummaryTable(result.data);
            }

            PauseForUser();
        }

        private void ProductsAboveAveragePrice()
        {
            ClearScreen();
            DisplayHeader("Products Above Average Price");

            var result = _productService.ProductsAboveAveragePrice();

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  Found {result.data.Count} product(s) above average price:\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void SearchAndFilterAdvanced()
        {
            ClearScreen();
            DisplayHeader("Advanced Search & Filter");

            Console.Write("  Enter search term (or press Enter to skip): ");
            string searchTerm = Console.ReadLine() ?? string.Empty;

            var filter = new ProductFilter();

            Console.Write("  Filter by category (or press Enter to skip): ");
            string category = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(category))
                filter.Category = category;

            Console.Write("  Filter by availability (Y/N/press Enter to skip): ");
            string available = Console.ReadLine();
            if (available?.ToLower() == "y")
                filter.Available = true;
            else if (available?.ToLower() == "n")
                filter.Available = false;

            if (GetPriceInput(out int min, out int max))
            {
                filter.MinPrice = min;
                filter.MaxPrice = max;
            }

            var result = _productService.SearchAndFilter(searchTerm, filter);

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  Found {result.data.Count} product(s):\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void PaginationSimulation()
        {
            ClearScreen();
            DisplayHeader("Pagination Simulation");

            Console.Write("  Enter page number (default 1): ");
            if (!int.TryParse(Console.ReadLine(), out int pageNumber) || pageNumber < 1)
                pageNumber = 1;

            Console.Write("  Enter page size (default 10): ");
            if (!int.TryParse(Console.ReadLine(), out int pageSize) || pageSize < 1)
                pageSize = 10;

            var result = _productService.PaginationSimulation(pageNumber, pageSize);

            if (!result.hasValue)
            {
                DisplayWarning(result.message);
            }
            else
            {
                Console.WriteLine($"\n  Page {pageNumber} - Showing {result.data.Count} product(s):\n");
                DisplayProductsTable(result.data);
            }

            PauseForUser();
        }

        private void DisplayProductsTable(List<Product> products)
        {
            Console.WriteLine("  ┌─────┬──────────────────────────┬─────────────┬───────────┬───────┬──────────┐");
            Console.WriteLine("  │ ID  │ Name                     │ Category    │ Price     │ Stock │ Available│");
            Console.WriteLine("  ├─────┼──────────────────────────┼─────────────┼───────────┼───────┼──────────┤");

            foreach (var product in products)
            {
                string name = product.Name.Length > 24 ? product.Name.Substring(0, 21) + "..." : product.Name.PadRight(24);
                string category = product.Category.Length > 11 ? product.Category.Substring(0, 8) + "..." : product.Category.PadRight(11);
                string price = $"{product.Price:N0}$".PadRight(9);
                string stock = product.StockQuantity.ToString().PadRight(6);
                string available = product.IsAvailable ? "YES" : "NO";

                Console.WriteLine($"  │ {product.ProductId,3} │ {name} │ {category} │ {price} │ {stock}│ {available,-8} │");
            }

            Console.WriteLine("  └─────┴──────────────────────────┴─────────────┴───────────┴──────────────────┘");
            Console.WriteLine();
        }

        private void DisplayProductSummaryTable(List<ProductSummary> products)
        {
            Console.WriteLine();
            Console.WriteLine("  ┌─────┬──────────────────────────┬──────────────┬───────────┬──────────────┐");
            Console.WriteLine("  │ ID  │ Name                     │ Category     │ Price     │ Stock        │");
            Console.WriteLine("  ├─────┼──────────────────────────┼──────────────┼───────────┼──────────────┤");

            foreach (var product in products)
            {
                string name = product.Name.Length > 24 ? product.Name.Substring(0, 21) + "..." : product.Name.PadRight(24);
                string category = product.Category.Length > 12 ? product.Category.Substring(0, 9) + "..." : product.Category.PadRight(12);
                string price = $"{product.Price:N0}$".PadRight(9);
                string stock = product.StockQuantity.ToString().PadRight(12);

                Console.WriteLine($"  │ {product.ProductId,3} │ {name} │ {category} │ {price} │ {stock} │");
            }

            Console.WriteLine("  └─────┴──────────────────────────┴──────────────┴───────────┴──────────────┘");
            Console.WriteLine();
        }

        private bool GetPriceInput(out int min, out int max)
        {
            min = 0;
            max = 0;

            Console.Write("  Enter minimum price: ");
            if (!int.TryParse(Console.ReadLine(), out min) || min < 0)
            {
                DisplayError("Invalid minimum price. Please enter a positive number.");
                return false;
            }

            Console.Write("  Enter maximum price: ");
            if (!int.TryParse(Console.ReadLine(), out max) || max < 0)
            {
                DisplayError("Invalid maximum price. Please enter a positive number.");
                return false;
            }

            if (min > max)
            {
                DisplayError("Minimum price cannot be greater than maximum price.");
                return false;
            }

            return true;
        }

        private void DisplayHeader(string title)
        {
            Console.WriteLine();
            string padding = new string('═', 50 - title.Length - 2);
            Console.WriteLine($"  {padding} {title} {padding}");
            Console.WriteLine();
        }

        private void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  ERROR: {message}\n");
            Console.ResetColor();
        }

        private void DisplayWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  {message}");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void DisplayGoodbye()
        {
            ClearScreen();
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine("║   Thank you for using Product Catalog!     ║");
            Console.WriteLine("║             Goodbye!                       ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        private void PauseForUser()
        {
            Console.Write("  Press any key to continue...");
            Console.ReadKey(intercept: true);
            Console.WriteLine();
        }

        private void ClearScreen()
        {
            Console.Clear();
        }
    }
}
