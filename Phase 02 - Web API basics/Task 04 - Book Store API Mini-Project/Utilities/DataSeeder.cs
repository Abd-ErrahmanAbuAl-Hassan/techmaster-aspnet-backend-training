using BookStoreApi.DTOs;
using BookStoreApi.Services;

namespace BookStoreApi.Utilities
{
    public static class DataSeeder
    {
        public static void SeedInitialData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var authorService = scope.ServiceProvider.GetRequiredService<IAuthorService>();
            var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();
            var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();

            // Seed Categories
            SeedCategories(categoryService);

            // Seed Authors
            SeedAuthors(authorService);

            // Seed Books
            SeedBooks(bookService);
        }

        private static void SeedCategories(ICategoryService categoryService)
        {
            var categories = new List<CreateCategoryRequest>
            {
                new CreateCategoryRequest
                {
                    Name = "Fiction",
                    Description = "Fictional stories and novels"
                },
                new CreateCategoryRequest
                {
                    Name = "Science Fiction",
                    Description = "Science fiction and futuristic tales"
                },
                new CreateCategoryRequest
                {
                    Name = "Non-Fiction",
                    Description = "Educational and factual books"
                },
                new CreateCategoryRequest
                {
                    Name = "Mystery",
                    Description = "Mystery and thriller novels"
                },
                new CreateCategoryRequest
                {
                    Name = "Technology",
                    Description = "Technology and programming books"
                }
            };

            foreach (var category in categories)
            {
                categoryService.CreateCategory(category);
            }
        }

        private static void SeedAuthors(IAuthorService authorService)
        {
            var authors = new List<CreateAuthorRequest>
            {
                new CreateAuthorRequest
                {
                    FullName = "J.K. Rowling",
                    Country = "United Kingdom",
                    BirthDate = new DateTime(1965, 7, 31)
                },
                new CreateAuthorRequest
                {
                    FullName = "George R. R. Martin",
                    Country = "United States",
                    BirthDate = new DateTime(1948, 9, 20)
                },
                new CreateAuthorRequest
                {
                    FullName = "Isaac Asimov",
                    Country = "United States",
                    BirthDate = new DateTime(1920, 1, 2)
                },
                new CreateAuthorRequest
                {
                    FullName = "Agatha Christie",
                    Country = "United Kingdom",
                    BirthDate = new DateTime(1890, 1, 15)
                },
                new CreateAuthorRequest
                {
                    FullName = "Robert C. Martin",
                    Country = "United States",
                    BirthDate = new DateTime(1952, 12, 5)
                }
            };

            foreach (var author in authors)
            {
                authorService.CreateAuthor(author);
            }
        }

        private static void SeedBooks(IBookService bookService)
        {
            var books = new List<CreateBookRequest>
            {
                new CreateBookRequest
                {
                    Title = "Harry Potter and the Philosopher's Stone",
                    ISBN = "9780747532699",
                    PublishedYear = 1997,
                    Price = 29.99m,
                    StockQuantity = 50,
                    AuthorId = 1,
                    CategoryId = 1
                },
                new CreateBookRequest
                {
                    Title = "Harry Potter and the Chamber of Secrets",
                    ISBN = "9780747538494",
                    PublishedYear = 1998,
                    Price = 29.99m,
                    StockQuantity = 45,
                    AuthorId = 1,
                    CategoryId = 1
                },
                new CreateBookRequest
                {
                    Title = "A Game of Thrones",
                    ISBN = "9780553103540",
                    PublishedYear = 1996,
                    Price = 34.99m,
                    StockQuantity = 30,
                    AuthorId = 2,
                    CategoryId = 1
                },
                new CreateBookRequest
                {
                    Title = "Foundation",
                    ISBN = "9780553293357",
                    PublishedYear = 1951,
                    Price = 24.99m,
                    StockQuantity = 20,
                    AuthorId = 3,
                    CategoryId = 2
                },
                new CreateBookRequest
                {
                    Title = "I, Robot",
                    ISBN = "9780553375077",
                    PublishedYear = 1950,
                    Price = 22.99m,
                    StockQuantity = 25,
                    AuthorId = 3,
                    CategoryId = 2
                },
                new CreateBookRequest
                {
                    Title = "Murder on the Orient Express",
                    ISBN = "9780062073556",
                    PublishedYear = 1934,
                    Price = 18.99m,
                    StockQuantity = 35,
                    AuthorId = 4,
                    CategoryId = 4
                },
                new CreateBookRequest
                {
                    Title = "Death on the Nile",
                    ISBN = "9780062073563",
                    PublishedYear = 1937,
                    Price = 18.99m,
                    StockQuantity = 0,
                    AuthorId = 4,
                    CategoryId = 4
                },
                new CreateBookRequest
                {
                    Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    ISBN = "9780132350884",
                    PublishedYear = 2008,
                    Price = 49.99m,
                    StockQuantity = 15,
                    AuthorId = 5,
                    CategoryId = 5
                },
                new CreateBookRequest
                {
                    Title = "The Pragmatic Programmer",
                    ISBN = "9780135957059",
                    PublishedYear = 2019,
                    Price = 59.99m,
                    StockQuantity = 10,
                    AuthorId = 5,
                    CategoryId = 5
                },
                new CreateBookRequest
                {
                    Title = "A Brief History of Time",
                    ISBN = "9780553380163",
                    PublishedYear = 1988,
                    Price = 25.99m,
                    StockQuantity = 22,
                    AuthorId = 3,
                    CategoryId = 3
                }
            };

            foreach (var book in books)
            {
                bookService.CreateBook(book);
            }
        }
    }
}