using BookStoreApi.DTOs;
using BookStoreApi.Models;
using BookStoreApi.Utilities;

namespace Task_04___Book_Store_API_Mini_Project.Services.Repositories
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