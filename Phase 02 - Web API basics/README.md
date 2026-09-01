# ASP.NET Core Web API — Mini Project Series

A series of four progressively more advanced **ASP.NET Core Web API** mini-projects, built to practice REST API fundamentals, layered architecture, validation, filtering/pagination, and clean, consistent API response design.

Each project is self-contained, uses **in-memory storage** (no database required), and follows a **Controller → Service → Utilities** layered structure with a shared-style `Result<T>` response wrapper.

---

## 📖 Table of Contents

- [Projects Overview](#-projects-overview)
- [Tech Stack](#-tech-stack)
- [Common Architecture](#-common-architecture)
- [Getting Started](#-getting-started)
- [Repository Structure](#-repository-structure)
- [Notes](#-notes)

---

## 📦 Projects Overview

### 1️⃣ Task 01 — REST Routing Drill Pack
A collection of small, focused endpoints for practicing core REST concepts: routing, route/query parameters, request bodies, HTTP status codes, and headers.

- Health check, echo, calculator, and unit-converter drills
- Grade calculator
- Full Notes CRUD with search and pagination
- Custom header reading (`X-Student-Name`)
- Dedicated endpoints demonstrating `200`, `201`, `204`, `400`, and `404` responses

### 2️⃣ Task 02 — Student Management API
Manages students enrolled in different tracks, with validation and statistics.

- Full Student CRUD, plus a dedicated status-update endpoint (`PATCH`)
- Search by name/email, filter by track and active status, with pagination
- Statistics endpoint (totals, active/inactive counts, breakdown by track)
- Egypt-specific email and phone number validation

### 3️⃣ Task 03 — Products & Categories API
Manages a product catalog organized into categories, with a composition layer joining the two.

- Full CRUD for Products and Categories
- `CatalogService` enriches categories with their related products
- Search/filter/pagination on both resources (price range, availability, low-stock threshold, etc.)
- Stock report endpoint (total value, per-category breakdown, low-stock/out-of-stock lists)

### 4️⃣ Task 04 — Book Store API
Manages a bookstore's books, authors, and categories.

- Full CRUD for Books, Authors, and Categories
- Search on title/ISBN, filter by category/author/availability, with pagination
- Business rules: unique ISBN, valid author/category references, inactive-category protection
- Inventory summary report (totals, value, breakdown by category and author)
- Automatic data seeding on startup

---

## 🛠 Tech Stack

- **.NET / ASP.NET Core Web API**
- **C#**
- In-memory data stores (static collections — no database required)
- Swagger / OpenAPI for interactive API testing

---

## 🏗 Common Architecture

All four projects share the same general design philosophy:

- **Controllers** — handle HTTP concerns only (routing, status code selection); no business logic
- **Services** — contain business logic, validation, and data operations
- **Utilities** — shared helpers such as the `Result<T>` response wrapper, filter objects, and validation helpers
- **Consistent responses** — every endpoint returns a predictable shape with a success flag, message, data payload, and error details, so consumers can handle success/failure uniformly across all four APIs

**Typical response shape:**

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { },
  "errors": []
}
```

---

## 🚀 Getting Started

Each project can be run independently.

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (7.0 or later recommended)
- An IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider

### Running a project

```bash
git clone https://github.com/<your-username>/<repo-name>.git
cd <repo-name>/<project-folder>
dotnet restore
dotnet run
```

Then open the Swagger UI to explore and test the endpoints:

```
https://localhost:<port>/swagger
```

---

## 🗂 Repository Structure

```
.
├── README.md                              # This file
├── task-01-rest-routing-drill-pack/
├── task-02-student-management-api/
├── task-03-products-categories-api/
└── task-04-book-store-api/
```

Each subfolder contains its own `README.md` with project-specific details: full endpoint list, request/response examples, and setup instructions.

---

## 📝 Notes

- These projects are **learning-focused** mini-projects, so they intentionally use in-memory storage rather than a database — all data resets on application restart.
- Each project increases in scope and complexity, moving from isolated routing drills (Task 01) to single-resource CRUD with validation (Task 02), to multi-resource composition (Task 03), to a more complete domain with cross-entity business rules and reporting (Task 04).
- See each project's individual README for detailed endpoint documentation and sample requests.