using BookStoreApi.DTOs;
using BookStoreApi.Models;
using BookStoreApi.Utilities;

namespace BookStoreApi.Services
{
    public interface IAuthorService
    {
        Result<List<AuthorResponse>> GetAllAuthors();
        Result<AuthorResponse> GetAuthorById(int id);
        Result<AuthorResponse> CreateAuthor(CreateAuthorRequest request);
        Result DeleteAuthor(int id);
        public bool AuthorExists(int id);
        public List<Author> GetAllAuthorsInternal();
    }
}