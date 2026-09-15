# 🗃️ EF Core Modeling Drill Pack

A RESTful Web API built with **ASP.NET Core** and **Entity Framework Core**, focused on practicing relational data modeling: one-to-one, one-to-many, and many-to-many-style relationships, navigation properties, cascading/restricted deletes, soft deletes, audit stamping, and paginated querying across related entities.

The domain models a small training academy: **Students**, **Instructors**, **Training Tracks**, **Enrollments**, and **Payment Summaries**.

---

## 📖 Table of Contents

- [Features](#-features)
- [Domain Model](#-domain-model)
- [Project Structure](#-project-structure)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Sample Requests](#-sample-requests)
- [Data Seeding](#-data-seeding)
- [Screenshots](#-screenshots)
- [Notes](#-notes)

---

## ✨ Features

- **EF Core relational modeling**
  - One-to-one: `Student` ↔ `StudentProfile`, `Enrollment` ↔ `PaymentSummary`
  - One-to-many: `Instructor` → `TrainingTrack`, `Student` → `Enrollment`, `TrainingTrack` → `Enrollment`
  - Composite/indexed relationship: unique-style index on `(StudentId, TrainingTrackId)` for enrollments
- **Soft delete** support for Students and Training Tracks (`IsDeleted` / `DeletedAt`), with an endpoint to list deleted students
- **Automatic audit stamping** (`CreatedAt` / `UpdatedAt`) via a shared `IAuditable` interface and `DbContext.SaveChanges` override
- **Pagination** on Students, Instructors, Training Tracks, and a track's enrolled students
- **Nested/related-data endpoints** — a student's tracks (with instructor names), an instructor's tracks, a track's paginated student list
- **Enrollment workflow** — duplicate-active-enrollment prevention, and a one-time payment summary per enrollment
- **Business rules**, e.g.:
  - An instructor must exist before a track can be created under them
  - A student can't have two *active* enrollments in the same track
  - A payment summary can only be created once per enrollment
  - Only the track's own instructor may update it (enforced via `403 Forbidden`)

---

## 🧬 Domain Model

| Entity            | Relationship                                                            |
|--------------------|----------------------------------------------------------------------------|
| `Student`          | Has one `StudentProfile`; has many `Enrollment`                              |
| `StudentProfile`    | Belongs to one `Student` (keyed by `SSN`)                                      |
| `Instructor`        | Has many `TrainingTrack`                                                        |
| `TrainingTrack`      | Belongs to one `Instructor`; has many `Enrollment`                                |
| `Enrollment`         | Belongs to one `Student` and one `TrainingTrack`; has one `PaymentSummary`          |
| `PaymentSummary`      | Belongs to one `Enrollment` (cascade delete)                                         |

---

## 🗂 Project Structure

```
task-01-ef-core-modeling-drill-pack/
├── README.md
├── Task_01___EF_Core_Modeling_Drill_Pack/
│   ├── Controllers/
│   │   ├── StudentsController.cs
│   │   ├── InstructorController.cs
│   │   ├── TrainingTracksController.cs
│   │   └── EnrollmentsController.cs
│   ├── Entities/
│   │   ├── Student.cs
│   │   ├── StudentProfile.cs
│   │   ├── Instructor.cs
│   │   ├── TrainingTrack.cs
│   │   ├── Enrollment.cs
│   │   ├── PaymentSummary.cs
│   │   ├── BaseEntity.cs
│   │   └── Enums/
│   │       └── EnrollmentStatus.cs
│   │       └── PaymentStatus.cs
│   ├── DTOs/
│   │   ├── CreateStudentRequest.cs
│   │   ├── UpdateStudentRequest.cs
│   │   ├── StudentListItemDto.cs
│   │   ├── TrackDto.cs
│   │   ├── CreateInstructorDTO.cs
│   │   ├── InstructorDTO.cs
│   │   ├── CreateTrackRequest.cs
│   │   ├── UpdateTrackRequest.cs
│   │   ├── TrackDetailsDto.cs
│   │   ├── TrackStudents.cs
│   │   ├── CreateEnrollmentDTO.cs
│   │   ├── EnrollmentDTO.cs
│   │   ├── CreatePaymentSummaryDTO.cs
│   │   └── PaginationResult.cs
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── SeedData.cs
│   └── Program.cs
```

---

## 🛠 Tech Stack

- **.NET / ASP.NET Core Web API**
- **C#**
- **Entity Framework Core** (relational database via `DbContext`)
- Swagger / OpenAPI for interactive API testing

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (9.0 or later recommended)
- A relational database supported by your configured EF Core provider (e.g. SQL Server, PostgreSQL)
- An IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider

### Installation & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/Abd-ErrahmanAbuAl-Hassan/techmaster-aspnet-backend-training.git
   cd "phase 03 - Real Backend Data Systems/Task 01 - EF Core Modeling Drill Pack"
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure your connection string**

   Update `appsettings.json` with your database connection string.

4. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Open the API documentation (Swagger UI)**
   ```
   https://localhost:7027/swagger/index.html
   ```

> On startup, `SeedData.SeedAsync` populates instructors, training tracks, students, and enrollments — but only if the `Students` table is currently empty.

---

## 📡 API Endpoints

### Students — `/api/students`

| Method | Endpoint                       | Description                                                      |
|--------|------------------------------------|------------------------------------------------------------------------|
| GET    | `/api/students`                    | Get all students, paginated                                              |
| GET    | `/api/students/{id}`               | Get a student by ID                                                        |
| POST   | `/api/students`                    | Create a student and their profile                                          |
| PUT    | `/api/students/{id}`               | Partially update a student and/or their profile                              |
| DELETE | `/api/students/{id}`               | Soft-delete a student                                                          |
| GET    | `/api/students/deleted`            | Get soft-deleted students, paginated                                            |
| GET    | `/api/students/{id}/tracks`        | Get the training tracks a student is enrolled in (with instructor names)         |

### Instructors — `/api/instructor`

| Method | Endpoint                       | Description                                |
|--------|------------------------------------|------------------------------------------------|
| GET    | `/api/instructor`                  | Get all instructors, paginated                   |
| GET    | `/api/instructor/{id}`             | Get an instructor by ID                             |
| POST   | `/api/instructor`                  | Create a new instructor                              |
| GET    | `/api/instructor/{id}/tracks`      | Get the training tracks led by an instructor           |

### Training Tracks — `/api/tracks`

| Method | Endpoint                            | Description                                                     |
|--------|-----------------------------------------|-----------------------------------------------------------------------|
| GET    | `/api/tracks`                          | Get all (non-deleted) tracks, paginated, with enrolled student counts    |
| GET    | `/api/tracks/{id}`                     | Get track details by ID                                                     |
| POST   | `/api/tracks?instructorId={id}`        | Create a new track under a given instructor                                    |
| PUT    | `/api/tracks/{id}`                     | Update a track (only its own instructor may update it)                            |
| DELETE | `/api/tracks/{id}`                     | Soft-delete a track                                                                  |
| GET    | `/api/tracks/{id}/students`            | Get a track's enrolled students, paginated                                            |

### Enrollments — `/api/enrollments`

| Method | Endpoint                                    | Description                                             |
|--------|--------------------------------------------------|--------------------------------------------------------------|
| POST   | `/api/enrollments`                               | Create a new enrollment                                        |
| GET    | `/api/enrollments/{id}`                          | Get an enrollment by ID                                          |
| POST   | `/api/enrollments/{id}/payment-summary`          | Create a payment summary for an enrollment (once per enrollment)  |

---

## 🔍 Sample Requests

**Create a student**
```http
POST /api/students
Content-Type: application/json

{
  "fName": "Sara",
  "lName": "Ibrahim",
  "email": "sara.ibrahim@example.com",
  "ssn": "29901010112345",
  "address": "Cairo, Egypt",
  "emergencyPhone": "01012345678",
  "birthOfDate": "1999-01-01"
}
```

**Create a training track**
```http
POST /api/tracks?instructorId=1
Content-Type: application/json

{
  "name": "ASP.NET Core Backend",
  "description": "REST APIs, EF Core, and authentication."
}
```

**Enroll a student in a track**
```http
POST /api/enrollments
Content-Type: application/json

{
  "studentId": 1,
  "trainingTrackId": 1,
  "status": "Active",
  "enrollmentDate": "2026-09-01"
}
```

**Add a payment summary**
```http
POST /api/enrollments/1/payment-summary
Content-Type: application/json

{
  "totalRequired": 5000,
  "totalPaid": 2500,
  "paymentStatus": "PartiallyPaid"
}
```

**Get a track's enrolled students (paginated)**
```http
GET /api/tracks/1/students?pageNumber=1&pageSize=10
```

---

## 🌱 Data Seeding

On startup, `SeedData.SeedAsync` seeds the database (only if it's empty) with:

- 2 instructors
- 3 training tracks (linked to the seeded instructors)
- 5 students
- 5 enrollments across the seeded students and tracks (including one `Completed` enrollment with a final grade)

---

## 🖼 Screenshots

Below are screenshots demonstrating the API in action (Swagger UI / Postman testing).

> 📌 **[View Screenshots on Google Drive](https://drive.google.com/drive/folders/1wGYd3fzJef-AGyS9imZ0jFJq8tqIRMFk?usp=drive_link)**

---
