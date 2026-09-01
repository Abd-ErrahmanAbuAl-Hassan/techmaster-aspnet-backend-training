# 📚 Book Store API

A RESTful Web API built with **ASP.NET Core** for managing a bookstore's books, authors, and categories. The project follows a clean, layered architecture (Controllers → Services → In-Memory Data Store) with a consistent response wrapper, validation, and support for searching, filtering, and pagination.

---

## 📖 Table of Contents

- [Features](#-features)
- [Project Structure](#-project-structure)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Sample Requests](#-sample-requests)
- [Response Format](#-response-format)
- [Data Seeding](#-data-seeding)
- [Screenshots](#-screenshots)
- [Notes](#-notes)

---

## ✨ Features

- **CRUD operations** for Books, Authors, and Categories
- **Search & Filtering** on books by title/ISBN, category, author, and availability
- **Pagination** for book listings
- **Business rule validation**, e.g.:
  - Unique ISBN per book
  - Valid, existing Author and Category references
  - Books cannot be created/updated under an inactive category
  - Stock quantity automatically drives availability status
- **Reports endpoint** — books summary with total inventory value, and breakdowns by category/author
- **Consistent API responses** via a generic `Result<T>` wrapper (`Success`, `Message`, `Data`, `Errors`)
- **Centralized validation** via a `ValidationHelper` utility
- **Initial data seeding** for categories, authors, and books on startup

---

## 🗂 Project Structure

```
task-04-book-store-api/
├── README.md
├── BookStoreApi/
│   ├── Controllers/
│   │   ├── AuthorsController.cs
│   │   ├── CategoriesController.cs
│   │   └── BooksController.cs
│   ├── Models/
│   │   ├── Author.cs
│   │   ├── Category.cs
│   │   └── Book.cs
│   ├── DTOs/
│   │   ├── CreateAuthorRequest.cs
│   │   ├── AuthorResponse.cs
│   │   ├── CreateCategoryRequest.cs
│   │   ├── CategoryResponse.cs
│   │   ├── CreateBookRequest.cs
│   │   ├── UpdateBookRequest.cs
│   │   ├── BookResponse.cs
│   │   ├── PaginatedResponse.cs
│   │   └── SummaryResponse.cs
│   ├── Services/
│   │   ├── IAuthorService.cs
│   │   ├── AuthorService.cs
│   │   ├── ICategoryService.cs
│   │   ├── CategoryService.cs
│   │   ├── IBookService.cs
│   │   └── BookService.cs
│   ├── Utilities/
│   │   ├── Result.cs
│   │   └── ValidationHelper.cs
│   └── Program.cs
```

---

## 🛠 Tech Stack

- **.NET / ASP.NET Core Web API**
- **C#**
- In-memory data store (static collections — no database required)
- Swagger / OpenAPI for interactive API testing

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (7.0 or later recommended)
- An IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider

### Installation & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/<your-username>/task-04-book-store-api.git
   cd task-04-book-store-api/BookStoreApi
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open the API documentation (Swagger UI)**
   ```
   https://localhost:<port>/swagger
   ```

> On startup, the application automatically seeds sample categories, authors, and books via `DataSeeder`.

---

## 📡 API Endpoints

### Authors — `/api/authors`

| Method | Endpoint            | Description          |
|--------|----------------------|-----------------------|
| GET    | `/api/authors`       | Get all authors       |
| GET    | `/api/authors/{id}`  | Get author by ID      |
| POST   | `/api/authors`       | Create a new author   |
| DELETE | `/api/authors/{id}`  | Delete author by ID   |

### Categories — `/api/categories`

| Method | Endpoint               | Description             |
|--------|-------------------------|---------------------------|
| GET    | `/api/categories`       | Get all categories        |
| GET    | `/api/categories/{id}`  | Get category by ID        |
| POST   | `/api/categories`       | Create a new category     |
| DELETE | `/api/categories/{id}`  | Delete category by ID     |

### Books — `/api/books`

| Method | Endpoint                     | Description                                         |
|--------|--------------------------------|-------------------------------------------------------|
| GET    | `/api/books`                   | Get all books (search, filter, paginate — see below)  |
| GET    | `/api/books/{id}`               | Get book by ID                                        |
| POST   | `/api/books`                    | Create a new book                                      |
| PUT    | `/api/books/{id}`               | Update an existing book                                 |
| DELETE | `/api/books/{id}`               | Delete book by ID                                       |
| GET    | `/api/books/reports/summary`    | Get inventory summary and statistics                    |

#### `GET /api/books` Query Parameters

| Parameter     | Type    | Description                                      |
|---------------|---------|---------------------------------------------------|
| `pageNumber`  | int     | Page number (default: `1`)                        |
| `pageSize`    | int     | Items per page (default: `10`)                     |
| `searchQuery` | string  | Searches title and ISBN                            |
| `categoryId`  | int     | Filter by category                                  |
| `authorId`    | int     | Filter by author                                    |
| `isAvailable` | bool    | Filter by availability status                       |

---

## 🔍 Sample Requests

**Create an author**
```http
POST /api/authors
Content-Type: application/json

{
  "fullName": "J. K. Rowling",
  "country": "United Kingdom",
  "birthDate": "1965-07-31"
}
```

**Create a book**
```http
POST /api/books
Content-Type: application/json

{
  "title": "Harry Potter and the Philosopher's Stone",
  "isbn": "9780747532699",
  "publishedYear": 1997,
  "price": 29.99,
  "stockQuantity": 50,
  "authorId": 1,
  "categoryId": 1
}
```

**Search and filter books**
```http
GET /api/books?searchQuery=harry&categoryId=1&isAvailable=true&pageNumber=1&pageSize=5
```

---

## 📦 Response Format

All endpoints return a consistent response shape using the `Result<T>` wrapper:

**Success**
```json
{
  "success": true,
  "message": "Books retrieved successfully",
  "data": { },
  "errors": []
}
```

**Failure**
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "Title is required",
    "Price must be positive"
  ]
}
```

---

## 🌱 Data Seeding

On application startup, `DataSeeder` populates the in-memory store with:

- 5 categories (Fiction, Science Fiction, Non-Fiction, Mystery, Technology)
- 5 authors
- 10 books distributed across the seeded categories and authors

This provides ready-to-use sample data for testing the endpoints immediately after running the project.

> ⚠️ Since data is stored in-memory (static lists), all data resets whenever the application restarts.

---

## 🖼 Screenshots

Below are screenshots demonstrating the API in action (Swagger UI / Postman testing).

> 📌 **[View Screenshots on Google Drive](https://drive.google.com/drive/folders/13dYxwz5hE2b3FOewDC93NmCiupVUr06Y?usp=drive_link)**

| Description               | Preview |
|----------------------------|---------|
| Swagger UI — All Endpoints | *(https://drive.google.com/file/d/1meBMEXr3pm92FUm3BojBNVm4gs7gs7UI/view?usp=drive_link)* |
| GET /api/books — Response  | *(https://drive.google.com/file/d/156q9-nLGd8KjH3NXYyjKRvtzO723yNCl/view?usp=drive_link)* |
| POST /api/books — Validation Error | *(https://drive.google.com/file/d/1V9sgYrSDPLuULmi2tQVfl3gpiKQTs8jH/view?usp=drive_link)* |
| GET /api/books/reports/summary | *(https://drive.google.com/file/d/1dyPS28sNI4fdLWjupUXkGCXZf9jO07EQ/view?usp=drive_link)* |

---

## 📝 Notes

- This project uses **in-memory storage** (static `List<T>` collections) rather than a database, so it's intended for learning/demo purposes.
- Business validation (e.g., ISBN uniqueness, valid author/category references, active category checks) is enforced in the service layer before any data mutation.
- The architecture separates concerns cleanly: **Controllers** handle HTTP, **Services** handle business logic, and **Utilities** (`Result`, `ValidationHelper`) provide shared, reusable behavior across the app.

---

### 👤 Author

Project developed as part of a mini-project task focused on building a layered, RESTful Web API with ASP.NET Core.