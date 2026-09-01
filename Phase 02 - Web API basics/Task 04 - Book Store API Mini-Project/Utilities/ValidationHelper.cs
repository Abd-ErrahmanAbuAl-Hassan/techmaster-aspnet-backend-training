using BookStoreApi.DTOs;

namespace BookStoreApi.Utilities
{
    public static class ValidationHelper
    {
        public static List<string> ValidateCreateBook(CreateBookRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Title))
                errors.Add("Title is required");

            if (string.IsNullOrWhiteSpace(request.ISBN))
                errors.Add("ISBN is required");

            if (request.Price <= 0)
                errors.Add("Price must be positive");

            if (request.StockQuantity < 0)
                errors.Add("Stock quantity cannot be negative");

            if (request.AuthorId <= 0)
                errors.Add("Author ID must be valid");

            if (request.CategoryId <= 0)
                errors.Add("Category ID must be valid");

            return errors;
        }
        public static List<string> ValidateUpdateBook(UpdateBookRequest request)
        {
            var errors = new List<string>();


            if (request.Price.HasValue && request.Price <= 0)
                errors.Add("Price must be positive");

            if (request.StockQuantity.HasValue && request.StockQuantity < 0)
                errors.Add("Stock quantity cannot be negative");

            if (request.AuthorId.HasValue && request.AuthorId <= 0)
                errors.Add("Author ID must be valid");

            if (request.CategoryId.HasValue && request.CategoryId <= 0)
                errors.Add("Category ID must be valid");

            if(request.PublishedYear.HasValue && request.PublishedYear <= 0 && request.PublishedYear > DateTime.Now.Year)
                errors.Add("Publish year must be valid");

            return errors;
        }

        public static List<string> ValidateCreateAuthor(CreateAuthorRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.FullName))
                errors.Add("Full name is required");

            return errors;
        }

        public static List<string> ValidateCreateCategory(CreateCategoryRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errors.Add("Category name is required");

            return errors;
        }
    }
}