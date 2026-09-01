using Task_03_Products_Categories_API.DTOs;
using Task_03_Products_Categories_API.Entities;
using Task_03_Products_Categories_API.Utilities;

namespace Task_03_Products_Categories_API.Services
{
    public class CatalogService
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;

        public CatalogService(
            CategoryService categoryService,
            ProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public Result<List<CategoryResponse>> GetCategoriesWithProducts(CFilter filter)
        {
            var categoriesResult = _categoryService.GetAllCategories(filter);

            if (!categoriesResult.Success)
                return new Result<List<CategoryResponse>>
                {
                    Success = false,
                    ErrorCode = categoriesResult.ErrorCode,
                    Message = categoriesResult.Message,
                    Errors = categoriesResult.Errors
                };

            var enrichedCategories = categoriesResult.Data.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                Products = _productService.GetAllProducts(new PFilter { Category = c.Name }).Data ?? new List<ProductResponse>()
            }).ToList();

            return new Result<List<CategoryResponse>>
            {
                Success = true,
                Message = $"Successfully retrieved ({enrichedCategories.Count}) categories with their products.",
                Data = enrichedCategories
            };
        }

        public Result<CategoryResponse> GetCategoryWithProducts(Guid categoryId)
        {
            var categoryResult = _categoryService.GetCategoryById(categoryId);

            if (!categoryResult.Success)
                return new Result<CategoryResponse>
                {
                    Success = false,
                    ErrorCode = categoryResult.ErrorCode,
                    Message = categoryResult.Message,
                    Errors = categoryResult.Errors
                };

            var category = categoryResult.Data;
            var categoryResponse = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                Products = _productService.GetAllProducts(new PFilter { Category = category.Name }).Data ?? new List<ProductResponse>()
            };

            return new Result<CategoryResponse>
            {
                Success = true,
                Message = "Category with products retrieved successfully.",
                Data = categoryResponse
            };
        }

       
    }
}
