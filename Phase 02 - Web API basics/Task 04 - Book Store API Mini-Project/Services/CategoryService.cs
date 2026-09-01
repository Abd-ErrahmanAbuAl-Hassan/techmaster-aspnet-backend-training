using BookStoreApi.DTOs;
using BookStoreApi.Models;
using BookStoreApi.Utilities;

namespace BookStoreApi.Services
{
    public class CategoryService : ICategoryService
    {
        private static List<Category> _categories = new List<Category>();
        private static int _categoryIdCounter = 1;

        public Result<List<CategoryResponse>> GetAllCategories()
        {
            try
            {
                var categoryResponses = _categories
                    .Select(c => new CategoryResponse
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        Description = c.Description,
                        IsActive = c.IsActive
                    })
                    .ToList();

                return Result<List<CategoryResponse>>.SuccessResult(categoryResponses, "Categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return Result<List<CategoryResponse>>.FailureResult("Failed to retrieve categories", ex.Message);
            }
        }

        public Result<CategoryResponse> GetCategoryById(int id)
        {
            try
            {
                var category = _categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null)
                    return Result<CategoryResponse>.FailureResult("Category not found", $"Category with ID {id} does not exist");

                var response = new CategoryResponse
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive
                };

                return Result<CategoryResponse>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                return Result<CategoryResponse>.FailureResult("Failed to retrieve category", ex.Message);
            }
        }

        public Result<CategoryResponse> CreateCategory(CreateCategoryRequest request)
        {
            try
            {
                var errors = ValidationHelper.ValidateCreateCategory(request);
                if (errors.Count > 0)
                    return Result<CategoryResponse>.FailureResult("Validation failed", errors);

                if (_categories.Any(c => c.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
                    return Result<CategoryResponse>.FailureResult("Duplicate category", "A category with this name already exists");

                var category = new Category
                {
                    CategoryId = _categoryIdCounter++,
                    Name = request.Name,
                    Description = request.Description,
                    IsActive = true
                };

                _categories.Add(category);

                var response = new CategoryResponse
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive
                };

                return Result<CategoryResponse>.SuccessResult(response, "Category created successfully");
            }
            catch (Exception ex)
            {
                return Result<CategoryResponse>.FailureResult("Failed to create category", ex.Message);
            }
        }

        public Result DeleteCategory(int id)
        {
            try
            {
                var category = _categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null)
                    return Result.FailureResult("Category not found", $"Category with ID {id} does not exist");

                _categories.Remove(category);
                return Result.SuccessResult("Category deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.FailureResult("Failed to delete category", ex.Message);
            }
        }

        public List<Category> GetAllCategoriesInternal()
        {
            return _categories;
        }

        public bool CategoryExists(int id)
        {
            return _categories.Any(c => c.CategoryId == id);
        }

        public bool IsCategoryActive(int id)
        {
            return _categories.FirstOrDefault(c => c.CategoryId == id)?.IsActive ?? false;
        }
    }
}