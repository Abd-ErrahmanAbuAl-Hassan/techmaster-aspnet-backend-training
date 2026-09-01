using BookStoreApi.DTOs;
using BookStoreApi.Models;
using BookStoreApi.Utilities;
using Task_04___Book_Store_API_Mini_Project.Services.Repositories;

namespace Task_04___Book_Store_API_Mini_Project.Services.Implementations
{
    public class BookService : IBookService
    {
        private static List<Book> _books = new List<Book>();
        private static int _bookIdCounter = 1;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;

        public BookService(IAuthorService authorService, ICategoryService categoryService)
        {
            _authorService = authorService;
            _categoryService = categoryService;
        }

        public Result<PaginatedResponse<BookResponse>> GetAllBooks(int pageNumber = 1, int pageSize = 10, string searchQuery = null, int? categoryId = null, int? authorId = null, bool? isAvailable = null)
        {
            try
            {
                var query = _books.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(b =>
                        b.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                        b.ISBN.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
                }

                if (categoryId.HasValue)
                    query = query.Where(b => b.CategoryId == categoryId);

                if (authorId.HasValue)
                    query = query.Where(b => b.AuthorId == authorId);

                if (isAvailable.HasValue)
                    query = query.Where(b => b.IsAvailable == isAvailable);

                var totalCount = query.Count();
                var books = query
                    .OrderByDescending(b => b.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var bookResponses = books.Select(b => MapBookToResponse(b)).ToList();
                var paginatedResponse = new PaginatedResponse<BookResponse>(bookResponses, pageNumber, pageSize, totalCount);

                return Result<PaginatedResponse<BookResponse>>.SuccessResult(paginatedResponse, "Books retrieved successfully");
            }
            catch (Exception ex)
            {
                return Result<PaginatedResponse<BookResponse>>.FailureResult("Failed to retrieve books", ex.Message);
            }
        }

        public Result<BookResponse> GetBookById(int id)
        {
            try
            {
                var book = _books.FirstOrDefault(b => b.BookId == id);
                if (book == null)
                    return Result<BookResponse>.FailureResult("Book not found", $"Book with ID {id} does not exist");

                var response = MapBookToResponse(book);
                return Result<BookResponse>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                return Result<BookResponse>.FailureResult("Failed to retrieve book", ex.Message);
            }
        }

        public Result<BookResponse> CreateBook(CreateBookRequest request)
        {
            try
            {
                var errors = ValidationHelper.ValidateCreateBook(request);
                if (errors.Count > 0)
                    return Result<BookResponse>.FailureResult("Validation failed", errors);

                if (_books.Any(b => b.ISBN.Equals(request.ISBN, StringComparison.OrdinalIgnoreCase)))
                    return Result<BookResponse>.FailureResult("Duplicate ISBN", "A book with this ISBN already exists");

                if (!_authorService.AuthorExists(request.AuthorId))
                    return Result<BookResponse>.FailureResult("Invalid author", $"Author with ID {request.AuthorId} does not exist");

                if (!_categoryService.CategoryExists(request.CategoryId))
                    return Result<BookResponse>.FailureResult("Invalid category", $"Category with ID {request.CategoryId} does not exist");

                if (!_categoryService.IsCategoryActive(request.CategoryId))
                    return Result<BookResponse>.FailureResult("Inactive category", "Cannot create book for inactive category");

                var book = new Book
                {
                    BookId = _bookIdCounter++,
                    Title = request.Title,
                    ISBN = request.ISBN,
                    PublishedYear = request.PublishedYear,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    AuthorId = request.AuthorId,
                    CategoryId = request.CategoryId,
                    IsAvailable = request.StockQuantity > 0,
                    CreatedAt = DateTime.UtcNow
                };

                _books.Add(book);

                var response = MapBookToResponse(book);
                return Result<BookResponse>.SuccessResult(response, "Book created successfully");
            }
            catch (Exception ex)
            {
                return Result<BookResponse>.FailureResult("Failed to create book", ex.Message);
            }
        }

        public Result<BookResponse> UpdateBook(int id, UpdateBookRequest request)
        {
            try
            {
                var book = _books.FirstOrDefault(b => b.BookId == id);
                if (book == null)
                    return Result<BookResponse>.FailureResult("Book not found", $"Book with ID {id} does not exist");

                var errors = ValidationHelper.ValidateUpdateBook(new UpdateBookRequest
                {
                    PublishedYear = request.PublishedYear,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    AuthorId = request.AuthorId,
                    CategoryId = request.CategoryId
                });

                if (errors.Count > 0)
                    return Result<BookResponse>.FailureResult("Validation failed", errors);

                if (request.ISBN != null && _books.Any(b => b.BookId != id && b.ISBN.Equals(request.ISBN, StringComparison.OrdinalIgnoreCase)))
                    return Result<BookResponse>.FailureResult("Duplicate ISBN", "A book with this ISBN already exists");

                if (request.AuthorId != null && !_authorService.AuthorExists(request.AuthorId.Value))
                    return Result<BookResponse>.FailureResult("Invalid author", $"Author with ID {request.AuthorId} does not exist");

                if (request.CategoryId != null && !_categoryService.CategoryExists(request.CategoryId.Value))
                    return Result<BookResponse>.FailureResult("Invalid category", $"Category with ID {request.CategoryId} does not exist");

                if (request.CategoryId != null && !_categoryService.IsCategoryActive(request.CategoryId.Value))
                    return Result<BookResponse>.FailureResult("Inactive category", "Cannot update book to inactive category");

                if (request.Title != null) book.Title = request.Title;
                if (request.ISBN != null) book.ISBN = request.ISBN;
                if (request.PublishedYear != null) book.PublishedYear = request.PublishedYear.Value;
                if (request.Price != null) book.Price = request.Price.Value;
                if (request.AuthorId != null) book.AuthorId = request.AuthorId.Value;
                if (request.CategoryId != null) book.CategoryId = request.CategoryId.Value;
                if (request.StockQuantity != null)
                {
                    book.StockQuantity = request.StockQuantity.Value;
                    book.IsAvailable = request.StockQuantity > 0;
                }

                var response = MapBookToResponse(book);
                return Result<BookResponse>.SuccessResult(response, "Book updated successfully");
            }
            catch (Exception ex)
            {
                return Result<BookResponse>.FailureResult("Failed to update book", ex.Message);
            }
        }

        public Result DeleteBook(int id)
        {
            try
            {
                var book = _books.FirstOrDefault(b => b.BookId == id);
                if (book == null)
                    return Result.FailureResult("Book not found", $"Book with ID {id} does not exist");

                _books.Remove(book);
                return Result.SuccessResult("Book deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.FailureResult("Failed to delete book", ex.Message);
            }
        }

        public Result<SummaryResponse> GetBooksSummary()
        {
            try
            {
                var summary = new SummaryResponse
                {
                    TotalBooks = _books.Count,
                    AvailableBooks = _books.Count(b => b.IsAvailable),
                    OutOfStockBooks = _books.Count(b => !b.IsAvailable),
                    TotalInventoryValue = _books.Sum(b => b.Price * b.StockQuantity)
                };

                var categories = _books
                    .GroupBy(b => b.CategoryId)
                    .ToDictionary(
                        g => _categoryService.GetCategoryById(g.Key).Data?.Name ?? "Unknown",
                        g => g.Count()
                    );

                summary.BooksByCategory = categories;

                var authors = _books
                    .GroupBy(b => b.AuthorId)
                    .ToDictionary(
                        g => _authorService.GetAuthorById(g.Key).Data?.FullName ?? "Unknown",
                        g => g.Count()
                    );

                summary.BooksByAuthor = authors;

                return Result<SummaryResponse>.SuccessResult(summary, "Summary retrieved successfully");
            }
            catch (Exception ex)
            {
                return Result<SummaryResponse>.FailureResult("Failed to generate summary", ex.Message);
            }
        }

        private BookResponse MapBookToResponse(Book book)
        {
            return new BookResponse
            {
                BookId = book.BookId,
                Title = book.Title,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                Price = book.Price,
                StockQuantity = book.StockQuantity,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                IsAvailable = book.IsAvailable,
                CreatedAt = book.CreatedAt
            };
        }
    }
}