# 🗄️ Task 06 - SQL & ERD Starter

Three business scenarios, each read and modeled into entities, tables, keys, relationships, and queries — practicing relational thinking before moving into EF Core. Each scenario below has its own schema (`DB schema creation.sql`), seed data (`SeedData.sql`), and query file (`Queries.sql`), plus an ERD covering its tables and relationships.

---

## 📖 Table of Contents

- [Scenario A — Library Management System](#scenario-a--library-management-system)
- [Scenario B — Simple Store & Orders System](#scenario-b--simple-store--orders-system)
- [Scenario C — Training Center Registration System](#scenario-c--training-center-registration-system)
- [Project Structure](#-project-structure)

---

## Scenario A — Library Management System

> 📌 **ERD:** [View ERD on Google Drive](https://drive.google.com/file/d/1sfxt4IYZmYtcE-zXoV1YCB-IMjLlEK6Q/view?usp=drive_link)

### Main Entities
- Authors
- Categories
- Books
- Members
- BorrowRecords

### Tables & Fields

| Table | Fields |
|-------|--------|
| **Authors** | `AuthorId` (PK), `FullName`, `BirthDate`, `Country` |
| **Categories** | `CategoryId` (PK), `Name` (unique), `Description` |
| **Books** | `BookId` (PK), `Title`, `PublishYear`, `ISBN` (unique), `AvailableCopies`, `AuthorId` (FK), `CategoryId` (FK) |
| **Members** | `MemberId` (PK), `FullName`, `Email` (unique), `PhoneNumber`, `JoinDate`, `IsActive` |
| **BorrowRecords** | `Id` (PK), `BorrowDate`, `DueDate`, `ReturnDate`, `Status`, `MemberId` (FK), `BookId` (FK) |

### Primary & Foreign Keys
- `Books.AuthorId` → `Authors.AuthorId`
- `Books.CategoryId` → `Categories.CategoryId`
- `BorrowRecords.MemberId` → `Members.MemberId`
- `BorrowRecords.BookId` → `Books.BookId`

### Relationships
- Author **has many** Books; Category **has many** Books (both one-to-many)
- Member **has many** BorrowRecords; Book **has many** BorrowRecords
- Members and Books are effectively **many-to-many**, resolved through the `BorrowRecords` junction table — a member can borrow many books over time, and a book can be borrowed by many members (at different times)

### Why I Designed It This Way
Authors and Categories are split into their own tables rather than stored as plain text on `Books`, so an author's details or a category's description live in exactly one place and stay consistent across every book that references them. `BorrowRecords` is the associative entity that resolves the natural many-to-many relationship between Members and Books — rather than cramming borrow/due/return dates onto `Books` (which would only support one active borrower at a time), each borrow event gets its own row with its own lifecycle. The `chk_BorrowRecords_Dates` and `chk_BorrowRecords_Status` check constraints keep that lifecycle honest at the database level: a due date can never precede the borrow date, and a record can only be marked "returned" once an actual return date exists. `AvailableCopies` is intentionally simple (a bit flag) for this starter scope rather than a full copy-tracking table, since the task calls for a simple relational model, not a full inventory system.

### SQL Queries
See [`Queries.sql`](./Scenario%20A%20-%20Library%20Management%20System/Queries.sql) for the full, runnable file. Included queries:
1. Select all books
2. Select all active members
3. Select books by category (by name, and by ID)
4. Count books per category
5. Select borrow records with member name and book title (**JOIN**)
6. Select overdue books (not yet returned, past due date)
7. Select borrowing history for one member
8. Select available books, joined with author and category names
9. Count how many books each author has
10. Select the top 5 most borrowed books

---

## Scenario B — Simple Store & Orders System

> 📌 **ERD:** [View ERD on Google Drive](https://drive.google.com/file/d/1myC-KlNZ7SIhDBeqAUlJl0PeEAZDD631/view?usp=drive_link)

### Main Entities
- Customers
- Categories
- Suppliers
- Products
- Orders
- OrderItems

### Tables & Fields

| Table | Fields |
|-------|--------|
| **Customers** | `CustomerId` (PK), `FullName`, `Email` (unique), `PhoneNumber`, `CreatedAt` |
| **Categories** | `CategoryId` (PK), `Name` (unique), `Description` |
| **Suppliers** | `SupplierId` (PK), `Name`, `PhoneNumber`, `Email` (unique) |
| **Products** | `ProductId` (PK), `Name`, `Price`, `StockQuantity`, `CategoryId` (FK), `SupplierId` (FK), `IsAvailable` |
| **Orders** | `OrderId` (PK), `CustomerId` (FK), `OrderDate`, `Status`, `TotalAmount` |
| **OrderItems** | `OrderItemId` (PK), `OrderId` (FK), `ProductId` (FK), `Quantity`, `UnitPrice` |

### Primary & Foreign Keys
- `Products.CategoryId` → `Categories.CategoryId`
- `Products.SupplierId` → `Suppliers.SupplierId`
- `Orders.CustomerId` → `Customers.CustomerId`
- `OrderItems.OrderId` → `Orders.OrderId`
- `OrderItems.ProductId` → `Products.ProductId`

### Relationships
- Customer **has many** Orders (one-to-many)
- Order **has many** OrderItems (one-to-many)
- Product **has many** OrderItems (one-to-many)
- Category **has many** Products (one-to-many)
- Supplier **has many** Products (one-to-many)
- Orders and Products are effectively **many-to-many**, resolved through the `OrderItems` junction table

### Why I Designed It This Way
`OrderItems` is the associative table that resolves the many-to-many relationship between Orders and Products — an order can contain many products, and a product can appear across many orders. Crucially, `OrderItems.UnitPrice` is stored as its own column rather than always looking up `Products.Price` live, so historical orders keep the price the customer actually paid even if the product's current price later changes. `Products` references both `Categories` and `Suppliers` directly as straightforward one-to-many relationships, since a product belongs to exactly one category and is sourced from exactly one supplier in this simplified model. Check constraints (`Price >= 0`, `StockQuantity >= 0`, `Quantity > 0`, and a constrained `Status` list on `Orders`) push basic data integrity down to the database layer rather than relying solely on application code to catch bad input.

### SQL Queries
See [`Queries.sql`](./Scenario%20B%20-%20Simple%20Store%20%26%20Orders%20System/Queries.sql) for the full, runnable file. Included queries:
1. Select all products
2. Select available products (in stock and marked available), with category and supplier names
3. Select products by category (by name, and by ID)
4. Select products with low stock (≤ 20 units)
5. Select orders for one customer
6. Select order details — customer, product, quantity, unit price, line total (**JOIN**)
7. Calculate total sales (excluding cancelled orders)
8. Count products per category
9. Select the top 10 best-selling products by quantity and revenue
10. Select suppliers with their products

---

## Scenario C — Training Center Registration System

> 📌 **ERD:** [View ERD on Google Drive](https://drive.google.com/file/d/1AFIWWPT_BodmTj60CjHSIk7vg5_YI2a6/view?usp=drive_link)

### Main Entities
- Students
- Instructors
- Tracks
- Registrations
- Payments

### Tables & Fields

| Table | Fields |
|-------|--------|
| **Students** | `StudentId` (PK), `FullName`, `Email` (unique), `PhoneNumber`, `CreatedAt` |
| **Instructors** | `InstructorId` (PK), `FullName`, `Email` (unique), `Specialization` |
| **Tracks** | `TrackId` (PK), `Title`, `Description`, `DurationWeeks`, `StartDate`, `InstructorId` (FK) |
| **Registrations** | `RegistrationId` (PK), `StudentId` (FK), `TrackId` (FK), `RegistrationDate`, `Status` |
| **Payments** | `PaymentId` (PK), `RegistrationId` (FK, unique), `Amount`, `PaymentDate`, `PaymentStatus` |

### Primary & Foreign Keys
- `Tracks.InstructorId` → `Instructors.InstructorId`
- `Registrations.StudentId` → `Students.StudentId`
- `Registrations.TrackId` → `Tracks.TrackId`
- `Payments.RegistrationId` → `Registrations.RegistrationId` (unique — enforces one-to-one)

### Relationships
- Instructor **has many** Tracks (one-to-many)
- Student **has many** Registrations; Track **has many** Registrations
- Students and Tracks are effectively **many-to-many**, resolved through the `Registrations` junction table
- Registration **has one** Payment (one-to-one, enforced by the `unique` constraint on `Payments.RegistrationId`)

### Why I Designed It This Way
`Registrations` is the associative entity that resolves the many-to-many relationship between Students and Tracks — a student can register for many tracks, and a track accepts many students — while also carrying its own lifecycle state (`Pending`, `Confirmed`, `Cancelled`, `Completed`) independent of the track itself. `Payments` is deliberately kept as its own table rather than adding payment columns directly onto `Registrations`, so payment history and status (`Pending`, `Paid`, `Failed`, `Refunded`) stay decoupled from registration status — a registration can exist and later be cancelled regardless of what happened with its payment. The `unique` constraint on `Payments.RegistrationId` enforces a strict one-to-one relationship, reflecting that each registration is billed exactly once in this model. `Tracks.InstructorId` is required (not nullable), reflecting the business rule that a track can't exist without an assigned instructor.

### SQL Queries
See [`Queries.sql`](./Scenario%20C%20-%20Training%20Center%20Registration%20System/Queries.sql) for the full, runnable file. Included queries:
1. Select all students
2. Select all tracks
3. Select students registered in a specific track (**JOIN**)
4. Count students per track
5. Select unpaid registrations (no payment row, or payment status not Paid)
6. Select tracks by instructor
7. Select registrations with payment status (**JOIN**)
8. Select tracks starting after a specific date
9. Count tracks per instructor
10. Select one student's full registration history, with track, instructor, and payment status

---

## 🗂 Project Structure

```
task-06-sql-erd-starter/
├── README.md
├── Scenario A - Library Management System/
│   ├── DB schema creation.sql
│   ├── SeedDate.sql
│   └── Queries.sql
├── Scenario B - Simple Store & Orders System/
│   ├── DB schema creation.sql
│   ├── SeedData.sql
│   └── Queries.sql
└── Scenario C - Training Center Registration System/
    ├── DB schema creation.sql
    ├── SeedData.sql
    └── Queries.sql
```