using Task_04_Product_Catalog_with_LINQ.Services;
using Task_04_Product_Catalog_with_LINQ.UI;

namespace Task_04___Product_Catalog_with_LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var productService = new ProductQueryService();

                var menu = new ConsoleMenu(productService);
                menu.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }
    }
}
