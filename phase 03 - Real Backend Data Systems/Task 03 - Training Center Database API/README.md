# 🏫 Training Center Database API

A RESTful Web API built with **ASP.NET Core** and **Entity Framework Core** for running a training center: students, instructors, training tracks, enrollments, payments, and management reporting. The project follows a clean, layered architecture (Controllers → Services → EF Core `DbContext`), with an interface-driven service layer, enrollment/payment workflows with state-transition rules, and a dedicated reporting module.

---

## 📖 Table of Contents

- [Features](#-features)
- [Domain Model](#-domain-model)
- [Project Structure](#-project-structure)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Sample Requests](#-sample-requests)
- [Response Format](#-response-format)
- [Business Rules & Workflows](#-business-rules--workflows)
- [Screenshots](#-screenshots)
- [Notes](#-notes)

---

## ✨ Features

- **CRUD & listing** for Students, Instructors, and Training Tracks
- **Enrollment workflow** — enroll a student in a track, track status transitions (`Draft → Active → Completed` / `Draft → Cancelled`), capacity and duplicate-enrollment checks
- **Payments workflow** — record payments against an enrollment, automatic `Pending` / `PartiallyPaid` / `Paid` status calculation, payment status transition validation, and auto-activation of an enrollment once fully paid
- **Reporting module** — dashboard summary, unpaid enrollments, track capacity, revenue summary, and revenue by track
- **Search & filtering** — students (name/email, active status), tracks (keyword, level, status, instructor), enrollments (status, track, student, payment status), payments (date range, status)
- **Pagination** on every list endpoint
- **Soft delete** for Students and Training Tracks, with audit timestamps (`CreatedAt` / `UpdatedAt`) stamped automatically by the `DbContext`
- **Authorization-style ownership checks** — only a track's own instructor can update or delete it (`403 Forbidden` otherwise)
- **Consistent API responses** via a generic `ApiResponse<T>` wrapper (`Success`, `Message`, `Data`, `Errors`, `ErrorCode`) and a shared `PagedResult<T>` for paginated data

---

## 🧬 Domain Model

| Entity            | Relationship                                                                 |
|---------------------|----------------------------------------------------------------------------------|
| `Student`            | Has many `Enrollment`                                                              |
| `Instructor`          | Has many `TrainingTrack`                                                             |
| `TrainingTrack`        | Belongs to one `Instructor`; has many `Enrollment`                                     |
| `Enrollment`            | Belongs to one `Student` and one `TrainingTrack`; has many `Payment`                      |
| `Payment`                | Belongs to one `Enrollment`                                                                 |

**Key constraints:** unique student/instructor email, unique track code, unique payment reference number.

---

## 🗂 Project Structure

```
task-03-training-center-database-api/
├── README.md
├── Task_03___Training_Center_Database_API/
│   ├── Controllers/
│   │   ├── StudentsController.cs
│   │   ├── InstructorsController.cs
│   │   ├── TracksController.cs
│   │   ├── EnrollmentsController.cs
│   │   ├── PaymentsController.cs
│   │   └── ReportsController.cs
│   ├── Entities/
│   │   ├── Student.cs
│   │   ├── Instructor.cs
│   │   ├── TrainingTrack.cs
│   │   ├── Enrollment.cs
│   │   └── Payment.cs
│   ├── DTOs/
│   │   ├── Requests/
│   │   │   ├── CreateStudentRequest.cs / UpdateStudentRequest.cs
│   │   │   ├── CreateInstructorRequest.cs / UpdateInstructorRequest.cs
│   │   │   ├── CreateTrackRequest.cs / UpdateTrackRequest.cs
│   │   │   ├── CreateEnrollmentRequest.cs / EnrollmentStatusUpdateRequest.cs
│   │   │   └── CreatePaymentRequest.cs / PaymentStatusUpdateRequest.cs
│   │   └── Responses/
│   │       ├── StudentListItemResponse.cs / StudentDetailsResponse.cs
│   │       ├── InstructorBasicResponse.cs
│   │       ├── TrackBasicResponse.cs / TrackDetailsResponse.cs
│   │       ├── EnrollmentSummaryResponse.cs / EnrollmentDetailsResponse.cs
│   │       ├── PaymentResponse.cs
│   │       └── Report*.cs (Dashboard, UnpaidEnrollment, TrackCapacity, RevenueSummary, RevenueByTrack)
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   ├── IStudentService.cs, IInstructorService.cs, ITrainingTrackService.cs
│   │   │   ├── IEnrollmentService.cs, IPaymentService.cs, IReportService.cs
│   │   ├── StudentService.cs
│   │   ├── InstructorService.cs
│   │   ├── TrainingTrackService.cs
│   │   ├── EnrollmentService.cs
│   │   ├── PaymentService.cs
│   │   └── ReportService.cs
│   ├── Utilities/
│   │   ├── ApiResponse.cs
│   │   ├── PagedResult.cs
│   │   ├── Enums/
│   │   │   ├── EnrollmentStatus.cs, PaymentStatus.cs, TrackLevel.cs, TrackStatus.cs
│   │   └── Interfaces/
│   │       ├── IAudiable.cs
│   │       └── ISoftDelete.cs
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
   cd "phase 03 - Real Backend Data Systems/Task 03 - Training Center Database API"
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure your connection string** in `appsettings.json`

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
   https://localhost:<port>/swagger
   ```

---

## 📡 API Endpoints

### Students — `/api/students`

| Method | Endpoint               | Description                                        |
|--------|--------------------------|--------------------------------------------------------|
| GET    | `/api/students`         | Get all students (search by name/email, filter by active status, paginated) |
| GET    | `/api/students/{id}`    | Get a student's details, including their enrollments      |
| POST   | `/api/students`         | Create a new student                                          |
| PUT    | `/api/students/{id}`    | Update a student's details                                       |
| DELETE | `/api/students/{id}`    | Soft-delete a student                                              |

### Instructors — `/api/instructors`

| Method | Endpoint                          | Description                                  |
|--------|---------------------------------------|--------------------------------------------------|
| GET    | `/api/instructors`                   | Get all active instructors, paginated              |
| GET    | `/api/instructors/{id}`              | Get an instructor by ID                              |
| GET    | `/api/instructors/{id}/tracks`       | Get the tracks led by an instructor, paginated         |
| POST   | `/api/instructors`                   | Create a new instructor                                  |
| PUT    | `/api/instructors/{id}`              | Update an instructor                                        |

### Training Tracks — `/api/tracks`

| Method | Endpoint                              | Description                                                     |
|--------|--------------------------------------------|-----------------------------------------------------------------------|
| GET    | `/api/tracks`                              | Get all tracks (keyword search, filter by level/status/instructor, paginated) |
| GET    | `/api/tracks/{id}`                         | Get track details                                                        |
| POST   | `/api/tracks`                              | Create a new track                                                           |
| PUT    | `/api/tracks/{id}`                         | Update a track (must be the track's own instructor)                             |
| DELETE | `/api/tracks/{id}?instructorId={id}`       | Soft-delete a track (must be the track's own instructor; blocked while it has active enrollments) |

### Enrollments — `/api/enrollments`

| Method | Endpoint                                     | Description                                                      |
|--------|---------------------------------------------------|------------------------------------------------------------------------|
| GET    | `/api/enrollments`                                | Get all enrollments (filter by status, track, student, payment status; paginated) |
| GET    | `/api/enrollments/{id}`                           | Get an enrollment's details, including its payments                       |
| POST   | `/api/enrollments`                                | Create a new enrollment (starts in `Draft` status)                          |
| PUT    | `/api/enrollments/{id}/status`                    | Update an enrollment's status (validated transitions only)                     |
| GET    | `/api/enrollments/student/{studentId}`            | Get a student's enrollments, paginated                                          |
| GET    | `/api/enrollments/track/{trackId}/students`       | Get the students enrolled in a track, paginated                                    |

### Payments — `/api/payments`

| Method | Endpoint                                  | Description                                                       |
|--------|------------------------------------------------|-------------------------------------------------------------------------|
| GET    | `/api/payments`                                | Get all payments (filter by date range and status, paginated)               |
| POST   | `/api/payments`                                | Record a payment against an enrollment                                        |
| GET    | `/api/payments/enrollment/{enrollmentId}`      | Get all payments for a given enrollment, paginated                              |
| PUT    | `/api/payments/{id}/status`                    | Update a payment's status (validated transitions only)                            |

### Reports — `/api/reports`

| Method | Endpoint                          | Description                                                          |
|--------|---------------------------------------|---------------------------------------------------------------------------|
| GET    | `/api/reports/dashboard-summary`     | Total active students, active/completed enrollments, total tracks, total revenue, unpaid amount |
| GET    | `/api/reports/unpaid-enrollments`    | Active enrollments that are not yet fully paid, paginated                      |
| GET    | `/api/reports/track-capacity`        | Each track's capacity vs. current active enrollment count, paginated              |
| GET    | `/api/reports/revenue-summary`       | Total, paid, partially-paid, and pending payment amounts                             |
| GET    | `/api/reports/revenue-by-track`      | Revenue, amount paid, and outstanding balance per track, paginated                     |

---

## 🔍 Sample Requests

**Create a student**
```http
POST /api/students
Content-Type: application/json

{
  "fName": "Mona",
  "lName": "Youssef",
  "email": "mona.youssef@example.com",
  "phoneNumber": "01012345678"
}
```

**Create a training track**
```http
POST /api/tracks
Content-Type: application/json

{
  "name": "Full-Stack .NET Development",
  "description": "ASP.NET Core, EF Core, and React.",
  "instructorId": 1,
  "capacity": 25,
  "price": 8000,
  "level": "Intermediate"
}
```

**Enroll a student in a track**
```http
POST /api/enrollments
Content-Type: application/json

{
  "studentId": 1,
  "trainingTrackId": 1
}
```

**Activate an enrollment**
```http
PUT /api/enrollments/1/status
Content-Type: application/json

{
  "status": "Active"
}
```

**Record a payment**
```http
POST /api/payments
Content-Type: application/json

{
  "enrollmentId": 1,
  "amount": 4000,
  "paymentMethod": "CreditCard",
  "notes": "First installment"
}
```

**Get the dashboard summary**
```http
GET /api/reports/dashboard-summary
```

---

## 📦 Response Format

All endpoints return a consistent response shape using the `ApiResponse<T>` wrapper, with paginated endpoints wrapping `Data` in a `PagedResult<T>`:

**Success (paginated)**
```json
{
  "success": true,
  "message": "Students retrieved successfully.",
  "errorCode": 0,
  "errors": null,
  "data": {
    "items": [ ],
    "totalCount": 42,
    "pageNumber": 1,
    "pageSize": 10
  }
}
```

**Failure**
```json
{
  "success": false,
  "message": "Validation errors.",
  "errorCode": 400,
  "errors": [
    "Student is already enrolled in this track."
  ],
  "data": null
}
```

---

## 🔐 Business Rules & Workflows

**Enrollment status transitions** (`EnrollmentStatus`): only the following are allowed —
`Draft → Active`, `Draft → Cancelled`, `Active → Completed`. Any other transition is rejected with a `400`.

**Payment status transitions** (`PaymentStatus`): only the following are allowed —
`Pending → PartiallyPaid`, `Pending → Paid`, `PartiallyPaid → Paid`.

**Enrollment creation** checks, in order: student exists → track exists → no existing non-cancelled enrollment for the same student/track → track has remaining capacity.

**Payment creation** checks: amount is positive → enrollment exists → enrollment isn't already fully paid → the new payment doesn't exceed the track's price. Once total payments reach the track price, the payment status is set to `Paid` and the related enrollment is automatically moved to `Active`.

**Track ownership**: updating or deleting a track requires the requesting `instructorId` to match the track's actual instructor, otherwise the API returns `403 Forbidden`. A track cannot be deleted while it has active enrollments.

---

## 🖼 Screenshots

Below are screenshots demonstrating the API in action (Swagger UI / Postman testing).

> 📌 **[View Screenshots on Google Drive](https://drive.google.com/drive/folders/1XQaA3kTmidFd2R9Q8iIU4oMZG21n-q1X?usp=drive_link)**
