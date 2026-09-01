# 🎓 Student Management API

A RESTful Web API built with **ASP.NET Core** for managing students enrolled in different tracks. The project follows a clean, layered architecture (Controllers → Services → In-Memory Data Store), with search/filtering, pagination, statistics, partial updates, and Egypt-specific validation rules for email and phone number.

---

## 📖 Table of Contents

- [Features](#-features)
- [Project Structure](#-project-structure)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Sample Requests](#-sample-requests)
- [Response Format](#-response-format)
- [Screenshots](#-screenshots)
- [Notes](#-notes)

---

## ✨ Features

- **CRUD operations** for Students
- **Search & Filtering** by name/email, track name, and active status
- **Pagination** on the students listing
- **Statistics endpoint** — total students, active/inactive counts, and student count grouped by track
- **Dedicated status update endpoint** (`PATCH`) with conflict protection when re-applying the same active/inactive status
- **Validation rules**, e.g.:
  - Required first name, last name, email, phone number, and track name on creation
  - Email format validation
  - Egyptian phone number format validation (`01[0,1,2,5]XXXXXXXX`)
- **Consistent API responses** via a generic `Result<T>` wrapper (`Success`, `Message`, `Data`, `Errors`, `ErrorCode`)

---

## 🗂 Project Structure

```
task-02-student-management-api/
├── README.md
├── Task_02___Student_Management_API/
│   ├── Controllers/
│   │   └── StudentsController.cs
│   ├── Entities/
│   │   └── Student.cs
│   ├── DTOs/
│   │   ├── CreateStudentRequest.cs
│   │   ├── UpdateStudentRequest.cs
│   │   ├── UpdateStudentStatusRequest.cs
│   │   ├── StudentResponse.cs
│   │   └── StudentStatsResponse.cs
│   ├── Services/
│   │   └── StudentService.cs
│   ├── Utilities/
│   │   ├── Result.cs
│   │   └── Filter.cs
│   └── Program.cs
```

---

## 🛠 Tech Stack

- **.NET / ASP.NET Core Web API**
- **C#**
- In-memory data store (static `List<Student>` — no database required)
- Swagger / OpenAPI for interactive API testing

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (7.0 or later recommended)
- An IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider

### Installation & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/<your-username>/task-02-student-management-api.git
   cd "task-02-student-management-api/Task_02___Student_Management_API"
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

---

## 📡 API Endpoints

### Students — `/api/students`

| Method | Endpoint                    | Description                                              |
|--------|--------------------------------|--------------------------------------------------------------|
| POST   | `/api/students/create`         | Create a new student                                           |
| GET    | `/api/students/all`            | Get all students (search, filter, paginate — see below)         |
| GET    | `/api/students/stats`          | Get student statistics                                            |
| GET    | `/api/students/{id}`           | Get a student by ID                                                |
| PUT    | `/api/students/{id}`           | Update a student's details                                          |
| PATCH  | `/api/students/{id}/status`    | Update a student's active/inactive status                            |
| DELETE | `/api/students/{id}/delete`    | Delete a student by ID                                                 |

#### `GET /api/students/all` Query Parameters

| Parameter    | Type    | Description                              |
|--------------|---------|---------------------------------------------|
| `Page`       | int     | Page number                                  |
| `PageSize`   | int     | Items per page (max 50)                      |
| `SearchTerm` | string  | Searches full name and email                  |
| `TrackName`  | string  | Filter by track name                            |
| `IsActive`   | bool    | Filter by active status                          |

---

## 🔍 Sample Requests

**Create a student**
```http
POST /api/students/create
Content-Type: application/json

{
  "fName": "Ahmed",
  "lName": "Mohamed",
  "email": "ahmed.mohamed@example.com",
  "phoneNumber": "01012345678",
  "trackName": ".NET Backend",
  "linkedInURL": "https://linkedin.com/in/ahmed-mohamed",
  "githubURL": "https://github.com/ahmed-mohamed"
}
```

**Search and filter students**
```http
GET /api/students/all?searchTerm=ahmed&trackName=.NET&isActive=true&page=1&pageSize=10
```

**Update a student's status**
```http
PATCH /api/students/{id}/status?newStatus=false
```

**Get student statistics**
```http
GET /api/students/stats
```

---

## 📦 Response Format

All endpoints return a consistent response shape using the `Result<T>` wrapper:

**Success**
```json
{
  "success": true,
  "message": "Successfully retrieve (12) students.",
  "errors": null,
  "data": { },
  "errorCode": 0
}
```

**Failure**
```json
{
  "success": false,
  "message": "Validation Errors.",
  "errors": [
    "Invalid email address.",
    "Phone number must be EGY phone number."
  ],
  "data": null,
  "errorCode": 400
}
```

---

## 🖼 Screenshots

Below are screenshots demonstrating the API in action (Swagger UI / Postman testing).

> 📌 **[View Screenshots on Google Drive](https://drive.google.com/drive/folders/1xv7klNLGjmgxOf4r76J6JpVVsRpFgl2r?usp=drive_link)**

| Description                            | Preview |
|-------------------------------------------|---------|
| Swagger UI — All Endpoints                | *(https://drive.google.com/file/d/1IBOO-uYk9o34YBzdq_PIPh20Q8K5xF9n/view?usp=drive_link)* |
| POST /api/students/create — Validation Error | *(https://drive.google.com/file/d/1psTKLPoRn8HlyKuX5VYBOjE6dRT8MzE8/view?usp=drive_link)* |
| GET /api/students/all — Search & Filter   | *(https://drive.google.com/file/d/1Vuh8TjFA8A4sORDCUVeDYYpmhKakrCB3/view?usp=drive_link)* |
| GET /api/students/stats — Response        | *(https://drive.google.com/file/d/14pxAHvaJ2CI9luR19u9GPzuXDTBPLW_i/view?usp=drive_link)* |

---

## 📝 Notes

- This project uses **in-memory storage** (a static `List<Student>`) rather than a database, so it's intended for learning/demo purposes.
- Phone number validation targets **Egyptian mobile numbers** (prefixes `010`, `011`, `012`, `015`), so this rule would need to be relaxed or made configurable for other regions.
- The status update endpoint is separated from the general update endpoint (`PATCH /{id}/status` vs. `PUT /{id}`) to keep status transitions explicit and to guard against redundant updates via a `409 Conflict` response.
- Error responses carry an `ErrorCode` (e.g. `400`, `404`, `409`, `500`) that controllers use to select the appropriate HTTP status code.

---

### 👤 Author

Project developed as part of a mini-project task focused on building a layered, RESTful Web API with ASP.NET Core.