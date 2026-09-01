# 🛒 Products & Categories API

A RESTful Web API built with **ASP.NET Core** for managing a product catalog organized into categories. The project follows a clean, layered architecture (Controllers → Services → In-Memory Data Store), with rich filtering/pagination, business-rule validation, and a dedicated catalog service that composes categories together with their products.

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

- **CRUD operations** for Products and Categories
- **Catalog view** — categories enriched with their related products (`CatalogService`)
- **Search & Filtering**
  - Categories: by name/description, active status
  - Products: by name/supplier, category, price range, availability, low-stock threshold
- **Pagination** on both category and product listings
- **Business rule validation**, e.g.:
  - Unique category names
  - Valid, existing category reference on product create/update
  - Price must be positive, stock quantity cannot be negative
  - Conflict protection when re-applying the same active/inactive status to a category
- **Stock report endpoint** — total stock value, per-category breakdown, low-stock and out-of-stock listings
- **Consistent API responses** via a generic `Result<T>` wrapper (`Success`, `Message`, `Data`, `Errors`, `ErrorCode`)
- **Initial data seeding** for categories and products on startup

---

## 🗂 Project Structure

```
task-03-products-categories-api/
├── README.md
├── Task_03_Products_Categories_API/
│   ├── Controllers/
│   │   ├── CategoriesController.cs
│   │   └── ProductsController.cs
│   ├── Entities/
│   │   ├── Category.cs
│   │   └── Product.cs
│   ├── DTOs/
│   │   ├── CreateCategoryRequest.cs
│   │   ├── UpdateCategoryRequest.cs
│   │   ├── CategoryResponse.cs
│   │   ├── CreateProductRequest.cs
│   │   ├── UpdateProductRequest.cs
│   │   ├── ProductResponse.cs
│   │   └── StockReportDto.cs (+ CategoryStockDto, LowStockProductDto, StockValueDto)
│   ├── Services/
│   │   ├── CategoryService.cs
│   │   ├── ProductService.cs
│   │   └── CatalogService.cs
│   ├── Utilities/
│   │   ├── Result.cs
│   │   ├── CFilter.cs
│   │   └── PFilter.cs
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

- [.NET SDK](https://dotnet.microsoft.com/download) (9.0 or later recommended)
- An IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider

### Installation & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/<your-username>/task-03-products-categories-api.git
   cd task-03-products-categories-api/Task_03_Products_Categories_API
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

> On startup, the application automatically seeds 4 categories and 15 products via each service's internal seed method.

---

## 📡 API Endpoints

### Categories — `/api/categories`

| Method | Endpoint               | Description                                          |
|--------|--------------------------|--------------------------------------------------------|
| GET    | `/api/categories`       | Get all categories, each enriched with its products    |
| GET    | `/api/categories/{id}`  | Get a single category (with its products) by ID        |
| POST   | `/api/categories`       | Create a new category                                   |
| PUT    | `/api/categories/{id}`  | Update a category (name, description, active status)    |
| DELETE | `/api/categories/{id}`  | Delete category by ID (Soft Delete)                     |

#### `GET /api/categories` Query Parameters

| Parameter    | Type    | Description                          |
|--------------|---------|----------------------------------------|
| `Page`       | int     | Page number                            |
| `PageSize`   | int     | Items per page (max 50)                |
| `SearchTerm` | string  | Searches category name and description |
| `IsActive`   | bool    | Filter by active status                |

### Products — `/api/products`

| Method | Endpoint                    | Description                             |
|--------|--------------------------------|--------------------------------------------|
| GET    | `/api/products`                | Get all products (search, filter, paginate) |
| GET    | `/api/products/{id}`           | Get product by ID                            |
| POST   | `/api/products`                | Create a new product                          |
| PUT    | `/api/products/{id}`           | Update an existing product                     |
| DELETE | `/api/products/{id}`           | Delete product by ID                             |
| GET    | `/api/products/reports/stock`  | Get stock report (default `lowStockThreshold=10`) |

#### `GET /api/products` Query Parameters

| Parameter            | Type    | Description                                   |
|-----------------------|---------|--------------------------------------------------|
| `Page`                | int     | Page number                                       |
| `PageSize`            | int     | Items per page (max 50)                           |
| `SearchTerm`          | string  | Searches product name and supplier name            |
| `Category`            | string  | Filter by category name                             |
| `MinPrice`/`MaxPrice` | int     | Filter by price range                               |
| `LowStockThreshold`   | int     | Filter products at or below this stock level        |
| `IsAvilable`          | bool    | Filter by availability status                        |

---

## 🔍 Sample Requests

**Create a category**
```http
POST /api/categories
Content-Type: application/json

{
  "name": "Electronics",
  "description": "Everything that works with electricity."
}
```

**Create a product**
```http
POST /api/products
Content-Type: application/json

{
  "name": "Laptop",
  "price": 45000,
  "stockQuantity": 5,
  "supplierName": "TechSupplier",
  "categoryId": "5c6b9f3e-1234-4a2b-9c3d-abcdef123456"
}
```

**Search and filter products**
```http
GET /api/products?searchTerm=laptop&category=Electronics&minPrice=1000&maxPrice=50000&page=1&pageSize=5
```

**Get the stock report**
```http
GET /api/products/reports/stock?lowStockThreshold=15
```

---

## 📦 Response Format

All endpoints return a consistent response shape using the `Result<T>` wrapper:

**Success**
```json
{
  "success": true,
  "message": "Successfully retrieve (15) products.",
  "errors": null,
  "data": { },
  "errorCode": 0
}
```

**Failure**
```json
{
  "success": false,
  "message": "Validation errors.",
  "errors": [
    "Price must be greater than 0.",
    "Stock quantity must be not negative."
  ],
  "data": null,
  "errorCode": 400
}
```

---

## 🌱 Data Seeding

On application startup, each service seeds its own in-memory store:

- **Categories:** Electronics, Furniture, Stationery, Accessories
- **Products:** 15 products distributed across the seeded categories (e.g. Laptop, Mouse, Keyboard, Office Chair, Desk, Notebook, Backpack), including some already out of stock to exercise availability filtering and reporting.

> ⚠️ Since data is stored in-memory (static lists), all data resets whenever the application restarts.

---

## 🖼 Screenshots

Below are screenshots demonstrating the API in action (Swagger UI / Postman testing).

> 📌 **[View Screenshots on Google Drive](https://drive.google.com/drive/folders/1Mh4G248sAifrEsD5hCX5BSPWQaiFRiyR?usp=drive_link)**

| Description                          | Preview |
|----------------------------------------|---------|
| Swagger UI — All Endpoints             | *(https://drive.google.com/file/d/1o1kHpeR8lzpxjrXZkExljK6v6bDOj5aj/view?usp=drive_link)* |
| GET /api/categories — Response         | *(https://drive.google.com/file/d/1UqvKSUsKVZWXdJU_iQQ-5LPtRg2-rlFf/view?usp=drive_link)* |
| POST /api/products — Validation Error  | *(https://drive.google.com/file/d/1flRYg7G6M2pOY-hgwlZMj5VXWyUlpWdS/view?usp=drive_link)* |
| GET /api/products/reports/stock        | *(https://drive.google.com/drive/folders/1F25uNzx-kLHuWoAHnEQ_FkQ2aut9kHtm?usp=drive_link)* |

---

## 📝 Notes

- This project uses **in-memory storage** (static `List<T>` collections) rather than a database, so it's intended for learning/demo purposes.
- `CatalogService` sits above `CategoryService` and `ProductService` to compose categories together with their associated products without coupling the two lower-level services to each other.
- Business validation (e.g., duplicate category names, valid category references, non-negative stock, positive price) is enforced in the service layer before any data mutation.
- Error responses carry an `ErrorCode` (e.g. `400`, `404`, `409`) that controllers use to select the appropriate HTTP status code (`BadRequest`, `NotFound`, `Conflict`).

---

### 👤 Author

Project developed as part of a mini-project task focused on building a layered, RESTful Web API with ASP.NET Core.