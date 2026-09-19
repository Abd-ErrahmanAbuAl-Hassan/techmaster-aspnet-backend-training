# 📊 Training Center API — Querying, Filtering, Pagination & Reports

This is **Task 04** in the Training Center series: the Task 03 database API (Students, Instructors, Training Tracks, Enrollments, Payments) extended with a full set of production-style querying, filtering, and reporting endpoints. The goal is to practice real API query patterns — search, filter, sort, paginate, and report — using projection DTOs instead of exposing raw entity graphs.

---

## 📖 Table of Contents

- [Task Summary](#-task-summary)
- [Query Spec Coverage](#-query-spec-coverage)
- [5 Important Queries Explained](#-5-important-queries-explained)
- [Project Structure](#-project-structure)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
- [Full API Reference](#-full-api-reference)
- [Response Format](#-response-format)
- [Screenshots](#-screenshots)
- [Notes](#-notes)

---

## 🎯 Task Summary

**Purpose:** train real API querying patterns — filtering, searching, sorting, and pagination — and build report endpoints that answer real business questions, returning projection DTOs rather than raw entity graphs.

**Required output for this task:**
- At least 15 of the 20 query/report specs implemented (all 20 implemented here)
- Swagger screenshots for 8 query endpoints
- A Postman collection with query examples
- This README, explaining 5 of the most important queries

---

## ✅ Query Spec Coverage

All 20 query specs from the task pack were implemented. The table below maps each spec to the actual route in this project — a few routes differ slightly from the spec sheet's suggested route, which is called out explicitly.

| # | Spec Name | Actual Route | Matches Spec Route? |
|---|------------|----------------|--------------------------|
| 1 | Search Students | `GET /api/students?search=mohamed` | ✅ |
| 2 | Filter Students By Status | `GET /api/students?isActive=true` (also supports `isDeleted`) | ✅ |
| 3 | Paged Students List | `GET /api/students?pageNumber=1&pageSize=10` | ✅ |
| 4 | Track Search | `GET /api/tracks?keyword=backend` | ✅ |
| 5 | Filter Tracks By Level | `GET /api/tracks?level=Beginner` | ✅ |
| 6 | Filter Tracks By Instructor | `GET /api/tracks?instructorId=2` | ✅ |
| 7 | Tracks With Available Seats | `GET /api/reports/tracks-with-available-seats` | ✅ |
| 8 | Enrollment List With Details | `GET /api/enrollments` | ✅ |
| 9 | Filter Enrollments By Status | `GET /api/enrollments?status=Pending` | ✅ |
| 10 | Student Enrollment History | `GET /api/enrollments/student/{studentId}` | ⚠️ Spec suggested `GET /api/students/{id}/enrollments`; implemented under the Enrollments controller instead |
| 11 | Track Students | `GET /api/enrollments/track/{trackId}/students` | ⚠️ Spec suggested `GET /api/tracks/{id}/students`; implemented under the Enrollments controller instead |
| 12 | Unpaid Enrollments | `GET /api/reports/unpaid-enrollments` | ✅ (see [Notes](#-notes) on which statuses are included) |
| 13 | Payments By Date Range | `GET /api/payments?startDate=2026-07-01&endDate=2026-07-31` | ⚠️ Params are named `startDate`/`endDate`, not `from`/`to` as in the spec |
| 14 | Revenue Summary | `GET /api/reports/revenue-summary` | ✅ |
| 15 | Revenue Per Track | `GET /api/reports/revenue-by-track` | ✅ |
| 16 | Top Tracks By Enrollment | `GET /api/reports/top-tracks?topCount=5` | ✅ (spec used `top`, this project uses `topCount`) |
| 17 | Instructor Workload | `GET /api/reports/instructors-workload` | ✅ (plural "instructors" vs. spec's "instructor") |
| 18 | Students Without Payments | `GET /api/reports/students-without-payments` | ✅ (see [Notes](#-notes) on scope) |
| 19 | Advanced Enrollment Filter | `GET /api/enrollments?trackId=1&status=Active&paymentStatus=Paid` | ✅ |
| 20 | Dashboard Summary | `GET /api/reports/dashboard-summary` | ✅ |

---

## 🔍 5 Important Queries Explained

### 1. Search Students (`GET /api/students?search=mohamed`)
Case-insensitive search across a student's full name, email, and phone number in a single query parameter, built with conditional `Where` + `Contains` clauses that only apply when `search` is non-empty. Returns `StudentListItemResponse` DTOs, not the raw `Student` entity.

### 2. Advanced Enrollment Filter (`GET /api/enrollments?trackId=1&status=Active&paymentStatus=Paid`)
Composes up to four optional filters (`status`, `trackId`, `studentId`, `paymentStatus`) on top of the enrollment listing by conditionally chaining `.Where()` calls onto an `IQueryable`, so only the filters the caller actually supplies affect the query. `paymentStatus` is applied after mapping to DTOs, since it's a derived value (calculated from related payments) rather than a column on `Enrollment` itself.

### 3. Tracks With Available Seats (`GET /api/reports/tracks-with-available-seats`)
A report-style query that loads tracks with their enrollments, computes each track's active enrollment count in memory, and returns only tracks where capacity exceeds that count — turning a simple capacity comparison into a genuinely useful "which tracks can I still enroll in" endpoint.

### 4. Revenue Per Track (`GET /api/reports/revenue-by-track`)
For each (non-deleted) track, aggregates total paid amount across all non-cancelled enrollments' payments, and reports theoretical revenue (`price × enrollment count`) alongside amount actually collected and the outstanding balance — the kind of report a training center's finance team would actually ask for.

### 5. Dashboard Summary (`GET /api/reports/dashboard-summary`)
A single endpoint that answers "how's the business doing right now" in one round trip: active student count, active/completed enrollment counts, total tracks, total revenue collected, and total unpaid amount outstanding — combining five separate aggregate queries into one response instead of forcing the client to make five calls.

---

## 🗂 Project Structure

```
task-04-querying-filtering-reporting/
├── README.md
├── Task_04_Querying_Filtering_Reporting/
│   ├── Controllers/
│   │   ├── StudentsController.cs
│   │   ├── InstructorsController.cs
│   │   ├── TracksController.cs
│   │   ├── EnrollmentsController.cs
│   │   ├── PaymentsController.cs
│   │   └── ReportsController.cs
│   ├── Entities/
│   │   ├── Student.cs, Instructor.cs, TrainingTrack.cs
│   │   ├── Enrollment.cs, Payment.cs
│   ├── DTOs/
│   │   ├── Requests/ (Create*/Update* request DTOs per entity)
│   │   └── Responses/
│   │       ├── StudentListItemResponse.cs / StudentDetailsResponse.cs
│   │       ├── InstructorBasicResponse.cs / InstructorWorkLoadResponse.cs
│   │       ├── TrackBasicResponse.cs / TrackDetailsResponse.cs / TrackMiniDetailsResponse.cs
│   │       ├── EnrollmentSummaryResponse.cs / EnrollmentDetailsResponse.cs / TrackEnrollmentStudents.cs
│   │       ├── PaymentResponse.cs / StudentsWithoutPayment.cs
│   │       └── Report*.cs (Dashboard, UnpaidEnrollment, TrackCapacity, TrackAvailableSeats, RevenueSummary, RevenueByTrack)
│   ├── Services/
│   │   ├── Interfaces/ (IStudentService, IInstructorService, ITrainingTrackService, IEnrollmentService, IPaymentService, IReportService)
│   │   ├── StudentService.cs, InstructorService.cs, TrainingTrackService.cs
│   │   ├── EnrollmentService.cs, PaymentService.cs
│   │   └── ReportService.cs
│   ├── Utilities/
│   │   ├── ApiResponse.cs, PagedResult.cs
│   │   └── Enums/ (EnrollmentStatus, PaymentStatus, TrackLevel, TrackStatus)
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
- Swagger / OpenAPI for interactive API testing and screenshots

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
   cd "phase 03 - Real Backend Data Systems/Task 04 Querying Filtering Reporting"
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

6. **Open Swagger UI**
   ```
   https://localhost:<port>/swagger
   ```

---

## 📡 Full API Reference

### Students — `/api/students`

| Method | Endpoint | Description |
|--------|-----------|----------------|
| GET | `/api/students?search=&isActive=&isDeleted=&pageNumber=&pageSize=` | Search/filter/paginate students |
| GET | `/api/students/{id}` | Student details, including enrollments and each enrollment's payment status |
| POST | `/api/students` | Create a student |
| PUT | `/api/students/{id}` | Update a student |
| DELETE | `/api/students/{id}` | Soft-delete a student |

### Instructors — `/api/instructors`

| Method | Endpoint | Description |
|--------|-----------|----------------|
| GET | `/api/instructors?pageNumber=&pageSize=` | List active instructors |
| GET | `/api/instructors/{id}` | Instructor by ID |
| GET | `/api/instructors/{id}/tracks?pageNumber=&pageSize=` | Tracks led by an instructor |
| POST | `/api/instructors` | Create an instructor |
| PUT | `/api/instructors/{id}` | Update an instructor |

### Training Tracks — `/api/tracks`

| Method | Endpoint | Description |
|--------|-----------|----------------|
| GET | `/api/tracks?keyword=&level=&status=&instructorId=&pageNumber=&pageSize=` | Search/filter/paginate tracks |
| GET | `/api/tracks/{id}` | Track details |
| POST | `/api/tracks` | Create a track |
| PUT | `/api/tracks/{id}` | Update a track (must be the track's own instructor) |
| DELETE | `/api/tracks/{id}?instructorId=` | Soft-delete a track (must be the track's own instructor; blocked with active enrollments) |

### Enrollments — `/api/enrollments`

| Method | Endpoint | Description |
|--------|-----------|----------------|
| GET | `/api/enrollments?status=&trackId=&studentId=&paymentStatus=&pageNumber=&pageSize=` | Combined/advanced enrollment filter |
| GET | `/api/enrollments/{id}` | Enrollment details, including its payments |
| POST | `/api/enrollments` | Create an enrollment (starts `Draft`, blocked by duplicate/capacity checks) |
| PUT | `/api/enrollments/{id}/status` | Update enrollment status (validated transitions only) |
| GET | `/api/enrollments/student/{studentId}?pageNumber=&pageSize=` | A student's enrollment history |
| GET | `/api/enrollments/track/{trackId}/students?pageNumber=&pageSize=` | Students enrolled in a track, with their enrollment status/date |

### Payments — `/api/payments`

| Method | Endpoint | Description |
|--------|-----------|----------------|
| GET | `/api/payments?startDate=&endDate=&status=&pageNumber=&pageSize=` | Payments filtered by date range and status |
| POST | `/api/payments` | Record a payment (auto-calculates `Pending`/`PartiallyPaid`/`Paid`) |
| GET | `/api/payments/enrollment/{enrollmentId}?pageNumber=&pageSize=` | Payments for one enrollment |
| PUT | `/api/payments/{id}/status` | Update payment status (validated transitions only) |

### Reports — `/api/reports`

| Method | Endpoint | Description |
|--------|-----------|----------------|
| GET | `/api/reports/dashboard-summary` | Students, enrollments, tracks, revenue, and unpaid amount in one call |
| GET | `/api/reports/unpaid-enrollments?pageNumber=&pageSize=` | Enrollments with an outstanding balance |
| GET | `/api/reports/track-capacity?pageNumber=&pageSize=` | Each track's capacity vs. active enrollment count |
| GET | `/api/reports/tracks-with-available-seats?pageNumber=&pageSize=` | Tracks that still have open seats |
| GET | `/api/reports/revenue-summary` | Total, paid, partially-paid, and pending payment amounts |
| GET | `/api/reports/revenue-by-track?pageNumber=&pageSize=` | Revenue, amount paid, and outstanding balance per track |
| GET | `/api/reports/top-tracks?topCount=5` | Tracks ranked by enrollment count |
| GET | `/api/reports/instructors-workload` | Each instructor's track count and active-student count |
| GET | `/api/reports/students-without-payments` | Students with a draft enrollment and no completed payment |

---

## 📦 Response Format

All endpoints return the same `ApiResponse<T>` wrapper used across the series, with paginated endpoints wrapping `Data` in a `PagedResult<T>`:

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

---

## 🖼 Screenshots

Swagger screenshots for the 8 required query endpoints, and the Postman collection export, go here.

> 📌 **[View Screenshots & Postman Collection on Google Drive](https://drive.google.com/drive/folders/1FEQv5Nb20NHNR2zWYJPhESrCJwTca4uy?usp=drive_link)**

