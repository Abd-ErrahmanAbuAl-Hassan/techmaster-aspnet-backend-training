using BookStoreApi.DTOs;
using BookStoreApi.Models;
using BookStoreApi.Utilities;
using Task_04___Book_Store_API_Mini_Project.Services.Repositories;

namespace Task_04___Book_Store_API_Mini_Project.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private static List<Author> _authors = new List<Author>();
        private static int _authorIdCounter = 1;

        public Result<List<AuthorResponse>> GetAllAuthors()
        {
            try
            {
                var authorResponses = _authors .Select(a => new AuthorResponse
                                               {
                                                   AuthorId = a.AuthorId,
                                                   FullName = a.FullName,
                                                   Country = a.Country,
                                                   BirthDate = a.BirthDate,
                                                   CreatedAt = a.CreatedAt
                                               }).ToList();

                return Result<List<AuthorResponse>>.SuccessResult(authorResponses, "Authors retrieved successfully");
            }
            catch (Exception ex)
            {
                return Result<List<AuthorResponse>>.FailureResult("Failed to retrieve authors", ex.Message);
            }
        }

        public Result<AuthorResponse> GetAuthorById(int id)
        {
            try
            {
                var author = _authors.FirstOrDefault(a => a.AuthorId == id);
                if (author == null)
                    return Result<AuthorResponse>.FailureResult("Author not found", $"Author with ID {id} does not exist");

                var response = new AuthorResponse
                {
                    AuthorId = author.AuthorId,
                    FullName = author.FullName,
                    Country = author.Country,
                    BirthDate = author.BirthDate,
                    CreatedAt = author.CreatedAt
                };

                return Result<AuthorResponse>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                return Result<AuthorResponse>.FailureResult("Failed to retrieve author", ex.Message);
            }
        }

        public Result<AuthorResponse> CreateAuthor(CreateAuthorRequest request)
        {
            try
            {
                var errors = ValidationHelper.ValidateCreateAuthor(request);
                if (errors.Count > 0)
                    return Result<AuthorResponse>.FailureResult("Validation failed", errors);

                var author = new Author
                {
                    AuthorId = _authorIdCounter++,
                    FullName = request.FullName,
                    Country = request.Country,
                    BirthDate = request.BirthDate,
                    CreatedAt = DateTime.UtcNow
                };

                _authors.Add(author);

                var response = new AuthorResponse
                {
                    AuthorId = author.AuthorId,
                    FullName = author.FullName,
                    Country = author.Country,
                    BirthDate = author.BirthDate,
                    CreatedAt = author.CreatedAt
                };

                return Result<AuthorResponse>.SuccessResult(response, "Author created successfully");
            }
            catch (Exception ex)
            {
                return Result<AuthorResponse>.FailureResult("Failed to create author", ex.Message);
            }
        }

        public Result DeleteAuthor(int id)
        {
            try
            {
                var author = _authors.FirstOrDefault(a => a.AuthorId == id);
                if (author == null)
                    return Result.FailureResult("Author not found", $"Author with ID {id} does not exist");

                _authors.Remove(author);
                return Result.SuccessResult("Author deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.FailureResult("Failed to delete author", ex.Message);
            }
        }

        public List<Author> GetAllAuthorsInternal()
        {
            return _authors;
        }

        public bool AuthorExists(int id)
        {
            return _authors.Any(a => a.AuthorId == id);
        }
    }
}