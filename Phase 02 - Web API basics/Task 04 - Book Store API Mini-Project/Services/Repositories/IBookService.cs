using BookStoreApi.DTOs;
using BookStoreApi.Utilities;

namespace Task_04___Book_Store_API_Mini_Project.Services.Repositories
{
    public interface IBookService
    {
        Result<PaginatedResponse<BookResponse>> GetAllBooks(int pageNumber = 1, int pageSize = 10, string searchQuery = null, int? categoryId = null, int? authorId = null, bool? isAvailable = null);
        Result<BookResponse> GetBookById(int id);
        Result<BookResponse> CreateBook(CreateBookRequest request);
        Result<BookResponse> UpdateBook(int id, UpdateBookRequest request);
        Result DeleteBook(int id);
        Result<SummaryResponse> GetBooksSummary();
    }
}