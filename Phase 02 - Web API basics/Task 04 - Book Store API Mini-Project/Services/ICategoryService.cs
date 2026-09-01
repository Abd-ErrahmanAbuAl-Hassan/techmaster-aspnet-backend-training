using BookStoreApi.DTOs;
using BookStoreApi.Models;
using BookStoreApi.Utilities;

namespace BookStoreApi.Services
{
    public interface ICategoryService
    {
        Result<List<CategoryResponse>> GetAllCategories();
        Result<CategoryResponse> GetCategoryById(int id);
        Result<CategoryResponse> CreateCategory(CreateCategoryRequest request);
        Result DeleteCategory(int id);
        public bool IsCategoryActive(int id);
        public bool CategoryExists(int id);
        public List<Category> GetAllCategoriesInternal();
    }
}