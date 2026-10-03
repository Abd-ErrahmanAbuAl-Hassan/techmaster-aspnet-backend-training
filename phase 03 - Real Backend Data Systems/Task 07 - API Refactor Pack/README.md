# 🔧 Task 07 — EF Core/API Refactor Pack

This task takes a deliberately bad Enrollments endpoint (`BadEnrollmentsController`) and refactors it into review-ready backend code (`GoodEnrollmentController` + `EnrollmentService`) — without changing what the API is fundamentally for. The original bad controller is kept in the project unchanged, side by side with the refactor, so the before/after is directly comparable.

---

## 📖 Table of Contents

- [Project Structure](#-project-structure)
- [Problems Found in the Original Code](#-problems-found-in-the-original-code)
- [Improvements Made in the Refactor](#-improvements-made-in-the-refactor)
- [Known Issues in the Refactor](#-known-issues-in-the-refactor-recommended-follow-ups)
- [API Reference — GoodEnrollmentController](#-api-reference--goodenrollmentcontroller)
- [Response Format](#-response-format)
- [Getting Started](#-getting-started)
- [Screenshots](#-screenshots)

---

## 🗂 Project Structure

```
task-07-api-refactor-pack/
├── README.md
├── Task_07___API_Refactor_Pack/
│   ├── Controllers/
│   │   ├── BadEnrollmentsController.cs    (left untouched — the "before")
│   │   └── GoodEnrollmentController.cs    (the refactor — the "after")
│   ├── Entities/
│   │   ├── Person.cs (abstract base: Student & Instructor)
│   │   ├── Student.cs, Instructor.cs
│   │   ├── TrainingTrack.cs, Enrollment.cs, Payment.cs
│   ├── DTOs/
│   │   ├── CreateEnrollmentRequest.cs, CreatePaymentRequest.cs
│   │   ├── EnrollmentDetailsResponse.cs, PaymentResponse.cs
│   │   ├── StudentListItemResponse.cs, TrackBasicResponse.cs
│   ├── Services/
│   │   ├── IEnrollmentService.cs
│   │   └── EnrollmentService.cs
│   ├── Utilities/
│   │   ├── ApiResponse.cs, PagedResult.cs
│   │   ├── PaymentCalculator.cs
│   │   └── Interfaces/ (IAudiable, ISoftDelete)
│   └── Data/
│       └── ApplicationDbContext.cs
```

`Person` is an abstract base class shared by `Student` and `Instructor` (common fields: name, email, phone, audit timestamps, `IsActive`), and `TrainingTrack`/`Enrollment` both implement `IAudiable` and `ISoftDelete` for consistent audit stamping and soft-delete behavior across the model.

---

## 🧨 Problems Found in the Original Code

`BadEnrollmentsController` talks directly to `ApplicationDbContext` with no service layer, and has the following issues:

1. **`GetAll` returns raw EF entities**, including navigation properties (`Student`, `TrainingTrack`, `Payments`) — this risks serializer cycles and leaks far more data than a client needs.
2. **No pagination or filtering on `GetAll`** — it loads and returns the entire `Enrollments` table on every call.
3. **`Create` binds the raw `Enrollment` entity directly from the request body** — a classic over-posting vulnerability: a client could set `Id`, `Status`, `CreatedAt`, or either foreign key to anything it wants.
4. **No input validation on `Create`** — a request with a non-existent `StudentId` or `TrainingTrackId` is accepted and saved as-is (and will likely only fail later with a raw FK-constraint database error).
5. **Duplicate active enrollments are allowed** — nothing stops the same student from being enrolled in the same track multiple times.
6. **Track capacity is ignored entirely** — a track can be over-enrolled with no limit.
7. **The `Pay` endpoint duplicates the enrollment-lookup query** and uses `FirstOrDefault` (synchronous) instead of `FirstOrDefaultAsync`, blocking a thread for no reason in an otherwise-async controller.
8. **`Pay` returns `Ok("not found")` — HTTP `200`, not `404`** — when the enrollment doesn't exist, the response still looks like success to anything checking the status code.
9. **No validation on the payment amount** — zero, negative, and wildly oversized amounts are all accepted without question.
10. **No overpayment or already-paid check** — nothing stops recording a second full payment against an enrollment that's already paid in full.
11. **`Delete` returns `Ok("missing")` — HTTP `200`, not `404`** — same wrong-status-code problem as `Pay`.
12. **`Delete` hard-deletes the row** (`_db.Enrollments.Remove(item)`), permanently destroying enrollment and payment history instead of preserving it.
13. **No consistent response shape** — responses are a mix of raw entities, bare strings (`"not found"`, `"deleted"`), and the occasional object, so a client can't parse responses uniformly.
14. **`Create` force-sets `Status = EnrollmentStatus.Active`** on every new enrollment, bypassing any "starts pending" business rule and hiding the fact the enrollment was never actually reviewed or confirmed.

---

## ✅ Improvements Made in the Refactor

`GoodEnrollmentController` now only handles HTTP concerns (status codes, request binding); all business logic moved into `EnrollmentService`, reached through the `IEnrollmentService` interface.

1. **Introduced a service layer** (`IEnrollmentService` / `EnrollmentService`) — the controller no longer touches `ApplicationDbContext` directly, so business rules are testable independent of HTTP.
2. **Everything is async** — `FirstOrDefaultAsync`, `CountAsync`, `SumAsync`, `ToListAsync` throughout; no blocking synchronous EF Core calls remain.
3. **`GetEnrollmentsAsync` is paginated** — `pageNumber`/`pageSize` drive `Skip`/`Take`, wrapped in a `PagedResult<T>` that also reports `TotalCount` and a computed `TotalPages`.
4. **Entities are projected into DTOs before leaving the service** — `EnrollmentDetailsResponse` nests `StudentListItemResponse`, `TrackBasicResponse`, and a list of `PaymentResponse`, instead of returning `Student`/`TrainingTrack`/`Payment` entities directly.
5. **`Create` now accepts a dedicated `CreateEnrollmentRequest` DTO** (`[Required] StudentId`, `[Required] TrainingTrackId`) instead of binding the `Enrollment` entity — closes the over-posting hole completely; the entity's own `Status`, `Id`, and audit fields are never client-settable.
6. **Duplicate-enrollment check added** — rejects creating a new enrollment if the student already has a non-cancelled enrollment in the same track (`400`, clear message).
7. **Track capacity is enforced** — active/completed enrollments are counted against `track.Capacity` and a full track returns `400` with *"Track capacity is full."*
8. **Track status is checked** — a `Closed` track is looked up successfully but explicitly rejected (*"This track is closed and cannot accept new enrollments."*); a track in any other non-published, non-closed state is reported as simply not found.
9. **Payment creation runs inside a database transaction** (`BeginTransactionAsync(IsolationLevel.Serializable)`, committed on success, rolled back on exception) — the payment insert and the enrollment-status update happen atomically.
10. **Payment input is validated** — amount must be positive, `PaymentMethod` must be a defined enum value, and the enrollment must exist, each with its own clear error message.
11. **Payment business rules are enforced** — a new payment against an already-fully-paid enrollment returns `409 Conflict`; a payment that would exceed the track's price returns `400`; a payment against a `Cancelled` enrollment is rejected outright — none of this was checked before.
12. **Correct, meaningful HTTP status codes everywhere** — `404` for not-found, `400` for validation failures, `409` for conflicts, and `201 Created` (via `CreatedAtAction`) on successful creation — no more "200 OK" wrapping an error string.
13. **`Delete` now soft-deletes** — sets `IsDeleted = true` and `DeletedAt = UtcNow` instead of removing the row, so enrollment and payment history survive a "deletion."
14. **A single, consistent `ApiResponse<T>` envelope** (`Success`, `Message`, `ErrorCode`, `Errors`, `Data`) is returned from every endpoint — a client can parse success and failure the same way across the whole controller.
15. **Payment-status calculation is centralized** in `PaymentCalculator.Calculate(enrollment)`, a single reusable static method, rather than duplicated inline arithmetic scattered across response mapping.

---

## ⚠️ Known Issues in the Refactor (Recommended Follow-ups)

Being transparent about what the refactor didn't fully fix — these are genuine bugs worth patching before calling this "done":

- **Track-capacity count has an operator-precedence bug.** In `CreateEnrollmentAsync`:
  ```csharp
  var enrolledCount = await _context.Enrollments
      .Where(e => e.TrainingTrackId == request.TrainingTrackId
               && e.Status == EnrollmentStatus.Completed
               || e.Status == EnrollmentStatus.Active)
      .CountAsync();
  ```
  Because `&&` binds tighter than `||`, this parses as `(TrainingTrackId == X && Status == Completed) || (Status == Active)` — it counts **every Active enrollment in the entire system**, not just the ones in this track, alongside Completed enrollments in this track specifically. The capacity check is effectively broken for any track once there's a meaningful number of active enrollments elsewhere. Fix: wrap the status check in its own parentheses —
  ```csharp
  .Where(e => e.TrainingTrackId == request.TrainingTrackId
           && (e.Status == EnrollmentStatus.Completed || e.Status == EnrollmentStatus.Active))
  ```
- **`GoodEnrollmentController.GetEnrollmentById` is `private` and throws `NotImplementedException`.** A `private` method decorated with `[HttpGet("{id}")]` isn't registered as an action by ASP.NET Core's controller convention, so the route effectively doesn't exist — and because `CreatedAtAction(nameof(GetEnrollmentById), ...)` relies on that action resolving to build the `Location` header, a successful `Create` call will throw at runtime (`No route matches the supplied values`) instead of returning `201 Created`. This needs to be made `public`, properly implemented against `IEnrollmentService`, and actually return an enrollment by ID.
- **Payment reference numbers are truncated to 20 characters** (`$"REF-{enrollment.Id}-{Guid.NewGuid():N}"[..20]`), which cuts short however much of the GUID segment doesn't fit after the enrollment ID — since the prefix length varies by enrollment ID, the amount of real randomness left varies too, which weakens the uniqueness guarantee a reference number is supposed to have (especially if `ReferenceNumber` has a unique constraint at the database level, as in earlier projects in this series).

---

## 📡 API Reference — GoodEnrollmentController

| Method | Endpoint | Description |
|--------|-------------|----------------|
| GET | `/api/goodenrollment?page=&pageSize=` | Paginated list of enrollments, each with student, track, and payment summary |
| GET | `/api/goodenrollment/{id}` | Get an enrollment by ID *(currently non-functional — see [Known Issues](#-known-issues-in-the-refactor-recommended-follow-ups))* |
| POST | `/api/goodenrollment` | Create an enrollment (validates student/track existence, duplicate enrollment, capacity, and track status) |
| POST | `/api/goodenrollment/pay` | Record a payment against an enrollment (validates amount, method, overpayment, and already-paid state) |
| DELETE | `/api/goodenrollment/{id}` | Soft-delete an enrollment |

---

## 📦 Response Format

```json
{
  "success": true,
  "message": "Enrollments retrieved successfully.",
  "errorCode": null,
  "errors": [],
  "data": {
    "items": [ ],
    "totalCount": 42,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5
  }
}
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (9.0 or later recommended)
- A relational database supported by your configured EF Core provider

### Run
```bash
cd Task_07___API_Refactor_Pack
dotnet restore
dotnet ef database update
dotnet run
```
Open Swagger at `https://localhost:<port>/swagger` — both `BadEnrollmentsController` and `GoodEnrollmentController` are visible there, so the two can be tested and compared side by side.

---

## 🖼 Screenshots

Before/after screenshots comparing `BadEnrollmentsController` and `GoodEnrollmentController` responses in Swagger or Postman go here.

> 📌 **[View Screenshots on Google Drive](https://drive.google.com/drive/folders/1vigOiYT9TXyVWe-UdnyUCBYTqtRavMm_?usp=drive_link)**


