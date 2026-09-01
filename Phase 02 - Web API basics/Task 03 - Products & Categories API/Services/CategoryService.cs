using Task_03_Products_Categories_API.DTOs;
using Task_03_Products_Categories_API.Entities;
using Task_03_Products_Categories_API.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Task_03_Products_Categories_API.Services
{
    public class CategoryService
    {
        private static List<Category> _categories = new List<Category>();

        public CategoryService()
        {
            InitializeDataSeed();
        }
        public Result<Category> Create(CreateCategoryRequest model)
        {
            if (model == null) return new Result<Category>
            {
                Success = false,
                ErrorCode = 400,
                Message = "Creation model is required",
                Errors = new List<string> { "model is null" }
            };

            if (_categories.Exists(c => c.Name == model.Name))
            {

                return new Result<Category>
                {
                    Success = false,
                    ErrorCode = 409,
                    Message = $"There is an existing category with this name'{model.Name}'",
                    Errors = new List<string> { "Duplicate Categories." }
                };
            }

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true
            };
            _categories.Add(category);

            return new Result<Category>
            {
                Success = true,
                Message = $"Category created successfully.",
                Data = category
            };
        }
        public Result<Category> Update(Guid id, UpdateCategoryRequest model)
        {
            if (model == null) return new Result<Category>
            {
                Success = false,
                ErrorCode = 400,
                Message = "Creation model is required",
                Errors = new List<string> { "model is null" }
            };

            if (_categories.Exists(c => c.Name == model.Name))
            {

                return new Result<Category>
                {
                    Success = false,
                    ErrorCode = 409,
                    Message = $"There is an existing category with this name'{model.Name}'",
                    Errors = new List<string> { "Duplicate Categories." }
                };
            }
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return new Result<Category>
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = $"Category not found.",
                    Errors = new List<string> { "Invalid category ID." }
                };
            }
            if (model.NewStatus is not null)
            {
                if(category.IsActive == model.NewStatus)
                    return model.NewStatus switch
                    {
                        true => new Result<Category>
                        {
                            Success = false,
                            ErrorCode = 409,
                            Message = $"Category is already active.",
                            Errors = new List<string> { "Conflict status update." }
                        },
                        false => new Result<Category>
                        {
                            Success = false,
                            ErrorCode = 409,
                            Message = $"Category is already inactive.",
                            Errors = new List<string> { "Conflict status update." }
                        }

                    };
            category.IsActive = (bool) model.NewStatus;
            }
            if(model.Name is not null) category.Name = model.Name;
            if(model.Description is not null) category.Description = model.Description;

            var index = _categories.IndexOf(category);
           
            _categories[index] = category;

            return new Result<Category>
            {
                Success = true,
                Message = $"Category updated successfully.",
                Data = category
            };

        }
        public Result<Category> Delete(Guid id)
        {
            if (!_categories.Exists(c => c.Id == id))
            {
                return new Result<Category>
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = $"Category not found.",
                    Errors = new List<string> { "Invalid category ID." }
                };
            }

            return new Result<Category> { Success = true, Message = "Category deleted successfully." };
        }
        public Result<List<Category>> GetAllCategories(CFilter filter)
        {
            if (!_categories.Any())
                return new Result<List<Category>>
                {
                    Success = false,
                    Message = "No Categories Exists, create the first one.",
                    ErrorCode = 404
                };

            var errors = new List<string>();
            var categories = _categories;

            if (filter.SearchTerm is not null)
            {
                categories = categories.Where(s => s.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)
                                            || s.Description.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (filter.IsActive is not null)
            {
                categories = categories.Where(s => s.IsActive == filter.IsActive).ToList();
            }
            var count = categories.Count;
            if (filter.PageSize is not null && filter.Page is not null)
            {
                if (filter.Page < 1) errors.Add("Page number must be greater than 1.");
                if (filter.PageSize < 1) errors.Add("Page size must be greater than 1.");
                if (filter.PageSize > 50) errors.Add("Page size must be less than 50.");
                if (errors.Any()) return new Result<List<Category>>
                {
                    Success = false,
                    Message = "Pagination parameters are not valid.",
                    Errors = errors,
                    ErrorCode = 400
                };

                categories = categories.Skip((int)((filter.Page - 1) * filter.PageSize))
                                   .Take((int)filter.PageSize).ToList();
            }

            if (!categories.Any())
                return new Result<List<Category>>
                {
                    Success = false,
                    Message = "No Categories found.",
                    ErrorCode = 404
                };

            return new Result<List<Category>>
            {
                Success = true,
                Message = $"Successfully retrieved ({count}) categories.",
                Data = categories
            };
        }
        public Category? GetCategoryByName(string name)
        {
            return _categories.FirstOrDefault(c => c.Name == name);
        }
        public Result<Category> GetCategoryById(Guid id)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return new Result<Category>
                {
                    Success = false,
                    ErrorCode = 404,
                    Message = "Category not found.",
                    Errors = new List<string> { "Invalid category ID." },
                };
            }

            return new Result<Category>
            {
                Success = true,
                Message = "Category retrieved successfully.",
                Data = category
            };
        }
        private void InitializeDataSeed()
        {
            var categories = new List<Category>()
            {
                new Category
                {
                    Name ="Electronics",
                    Description = "Everything work with electricity.",
                    IsActive = true,
                },
                new Category
                {
                    Name ="Furniture",
                    Description = "Everything made by wood.",
                    IsActive = true,
                },
                new Category
                {
                    Name ="Stationery",
                    Description = "Everything from library staff.",
                    IsActive = true,
                },
                new Category
                {
                    Name ="Accessories",
                    Description = "Everything to make your PC setup to be legendary.",
                    IsActive = true,
                }
            };

            _categories.AddRange(categories);
        }
    }
}
