# 🛡️ Training Center API — Business Rules & Data Integrity

This is **Task 05** in the Training Center series: the Task 04 querying/reporting API hardened with explicit business rules so it protects business correctness — not just CRUD. Every rule below is enforced in the **service layer**, returns **`400 Bad Request`** with a clear message on violation, and can be reproduced with a single request in Swagger or Postman.

---

## 📖 Table of Contents

- [Task Summary](#-task-summary)
- [Installation & Run](#-Installation-&-Run)
- [Business Rules](#-business-rules)
  - [Student Rules](#student-rules)
  - [Track Rules](#track-rules)
  - [Enrollment Rules](#enrollment-rules)
  - [Payment Rules](#payment-rules)
- [Applying These Changes](#-applying-these-changes)
- [Reproducing the Required Invalid Test Cases](#-reproducing-the-required-invalid-test-cases)
- [Screenshots](#-screenshots)

---

## 🎯 Task Summary

**Purpose:** stop the API from being "dumb CRUD" and make it actively protect business correctness — invalid operations must be rejected with a clear `400`, not silently accepted or left to a database constraint to throw a raw `500`.

**Required output for this task:**
- Business rules implemented in the service layer (not controllers, not the database)
- Invalid operations return `400 Bad Request` with clear error messages
- This README's Business Rules section
- Postman tests covering invalid scenarios
- A mentor can reproduce every rule's behavior from the documentation below

---
### Installation & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/Abd-ErrahmanAbuAl-Hassan/techmaster-aspnet-backend-training.git
   cd "phase 03 - Real Backend Data Systems/Task 05 - Business Rules & Data Integrity"
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
## 📐 Business Rules

Every rule is enforced in a service method, before any data is written. Where a rule needed a small entity/DTO/enum addition (a field or enum value that didn't exist yet), that's called out — see [Applying These Changes](#-applying-these-changes) for the exact additions.

### Student Rules

| Rule | Enforced In | Behavior |
|------|--------------|-------------|
| Email must be unique | `StudentService.CreateStudentAsync`, `UpdateStudentAsync` | `400` — *"Email already exists."* |
| FullName is required | `StudentService.ValidateStudentRequest` (`FName`/`LName`) | `400` — *"First name is required."* / *"Last name is required."* |
| Student cannot be hard-deleted by default | `StudentService.DeleteStudentAsync` | Sets `IsActive = false`, `IsDeleted = true`, `DeletedAt = UtcNow` — the row is never removed |
| Inactive or deleted students cannot receive a new active enrollment unless explicitly allowed | `EnrollmentService.CreateEnrollmentAsync` | `400` — *"Student is inactive or deleted and cannot be enrolled. Set AllowInactiveStudent=true to override."* Requires a new `AllowInactiveStudent` flag on the request (defaults to `false`) |
| Deleted student should not appear in normal list endpoints | `StudentService.GetStudentsAsync` | The default listing (no `isDeleted` query param) now excludes soft-deleted students; pass `isDeleted=true` to see them explicitly |

### Track Rules

| Rule | Enforced In | Behavior |
|------|--------------|-------------|
| Title is required | `TrainingTrackService.CreateTrackAsync` | `400` — *"Track name is required."* |
| Code must be unique | `TrainingTrackService.CreateTrackAsync` | The auto-generated code is checked against existing codes and regenerated on collision (up to 10 attempts) instead of letting a raw DB unique-constraint exception surface as a `500` |
| Capacity must be greater than 0 | `CreateTrackAsync` **and** `UpdateTrackAsync` | `400` — *"Capacity must be greater than zero."* (Previously, an invalid capacity on **update** was silently ignored instead of rejected — now fixed) |
| StartDate must be before EndDate | `CreateTrackAsync` **and** `UpdateTrackAsync` | `400` — *"StartDate must be before EndDate."* Requires new `StartDate`/`EndDate` fields (see below) |
| Instructor is required | `CreateTrackAsync` | `400` — *"Instructor not found."* |
| Track cannot exceed capacity | `EnrollmentService.CreateEnrollmentAsync` | `400` — *"Track capacity is full."* Capacity is compared against active (non-cancelled) enrollment count |
| Closed track cannot accept new enrollments | `EnrollmentService.CreateEnrollmentAsync` | `400` — *"This track is closed and cannot accept new enrollments."* |

### Enrollment Rules

| Rule | Enforced In | Behavior |
|------|--------------|-------------|
| Student cannot have two active enrollments in the same track | `EnrollmentService.CreateEnrollmentAsync` | `400` — *"Student is already enrolled in this track."* (checks for any non-cancelled enrollment, not just Active, so you can't re-enroll around a Pending one either) |
| Enrollment starts as Pending by default | `EnrollmentService.CreateEnrollmentAsync` | New enrollments are created with `EnrollmentStatus.Pending` |
| Enrollment can become Active after payment or status allows it | `EnrollmentService.UpdateEnrollmentStatusAsync`, `PaymentService.CreatePaymentAsync` | A `Paid` payment auto-transitions the enrollment to `Active`; `Pending → Active` is also a valid manual transition |
| Completed enrollment cannot be cancelled directly | `EnrollmentService.IsValidStatusTransition` | Only `Pending → Active`, `Pending → Cancelled`, and `Active → Completed` are valid transitions — `Completed → Cancelled` is not in the table, so it's rejected with `400` |
| Cancelled enrollment should not count in active capacity | `EnrollmentService.CreateEnrollmentAsync` | Both the duplicate-enrollment check and the capacity check explicitly exclude `Cancelled` enrollments |

### Payment Rules

| Rule | Enforced In | Behavior |
|------|--------------|-------------|
| Payment amount must be positive | `PaymentService.CreatePaymentAsync` | `400` — *"Amount must be greater than zero."* |
| Payment cannot exceed remaining amount | `PaymentService.CreatePaymentAsync` | `400` — *"Payment amount exceeds track price."* (overpayment is rejected outright, not silently capped) |
| Payment has status Pending, Paid, PartiallyPaid, Failed, or Refunded | `PaymentService.IsValidStatusTransition` | Transition table now includes `Pending → Failed`, `PartiallyPaid → Failed`, and `Paid → Refunded`, in addition to the existing paid-path transitions |
| Only Paid payments count in revenue reports | `ReportService.GetRevenueSummaryAsync`, `GetRevenueByTrackAsync`, `EnrollmentService.CalculatePaymentStatus` / `MapToEnrollmentDetailsResponse` | "Total paid" and "total revenue" calculations now filter to `Paid`/`PartiallyPaid` only — a Failed or Refunded payment's amount is never counted as money received |
| Failed payment should not activate enrollment | `PaymentService.CreatePaymentAsync` | The enrollment-activation call only fires `if (status == PaymentStatus.Paid)` — a Failed payment (set via a later status update) never triggers it |

---

## 🛠 Applying These Changes

The four services below were rewritten in full and are ready to drop in:

- `StudentService.cs`
- `TrainingTrackService.cs`
- `EnrollmentService.cs`
- `PaymentService.cs`

`ReportService.cs` needs three small, precise edits — see **`ReportService-PATCH.md`** for exact find/replace snippets (not a full rewrite, to avoid risking an unrelated change to code that already works).

These service changes depend on a few small, **additive** entity/DTO/enum changes that don't exist in the codebase yet:

| File | Addition | Why |
|------|-------------|--------|
| `TrainingTrack` entity | `DateTime StartDate`, `DateTime EndDate` | Track Rules: StartDate must be before EndDate |
| `CreateTrackRequest` | `DateTime StartDate`, `DateTime EndDate` | Same |
| `UpdateTrackRequest` | `DateTime? StartDate`, `DateTime? EndDate` | Same, optional on update |
| `CreateEnrollmentRequest` | `bool AllowInactiveStudent = false` | Student Rules: override for enrolling an inactive/deleted student |
| `EnrollmentStatus` enum | Rename `Draft` → `Pending` (keep the same underlying number, e.g. `0`) | Enrollment Rules: new enrollments must show status `Pending`, not `Draft` |
| `PaymentStatus` enum | Add `Failed`, `Refunded` (append as new values, e.g. `3` and `4`) | Payment Rules: all four required statuses must exist |
| `TrackStatus` enum | Confirm a `Closed` member exists (add if missing) | Track Rules: closed tracks reject new enrollments |

**Why these are safe:** the `EnrollmentStatus` rename keeps the same integer value, so existing rows in the database are unaffected (EF Core stores enums as integers by default here). The `PaymentStatus` additions and the `TrainingTrack`/DTO date fields are purely additive — nothing existing changes shape. The `Draft` → `Pending` rename was applied everywhere it's referenced in `EnrollmentService.cs` and the patched parts of `ReportService.cs`; if `Draft` is referenced anywhere else in the solution (e.g. seed data), search-and-replace it too.

---

## ✅ Reproducing the Required Invalid Test Cases

### Student
```http
# Duplicate email -> 400
POST /api/students
{ "fName": "Ahmed", "lName": "Ali", "email": "existing@example.com", "phoneNumber": "01012345678" }

# Delete -> IsDeleted true, student still exists in DB
DELETE /api/students/1

# List -> deleted student not included
GET /api/students
```

### Track
```http
# Capacity 0 -> 400
POST /api/tracks
{ "name": "Test Track", "description": "...", "instructorId": 1, "capacity": 0, "price": 100, "startDate": "2026-10-01", "endDate": "2026-12-01" }

# Enroll into a full track -> 400
POST /api/enrollments
{ "studentId": 5, "trainingTrackId": 2 }   # where track 2 is already at capacity

# Create without instructor -> 400
POST /api/tracks
{ "name": "Test Track", "description": "...", "instructorId": 9999, "capacity": 10, "price": 100, "startDate": "2026-10-01", "endDate": "2026-12-01" }
```

### Enrollment
```http
# Duplicate active enrollment -> 400
POST /api/enrollments
{ "studentId": 1, "trainingTrackId": 1 }   # called twice for the same pair

# New enrollment status -> Pending
POST /api/enrollments
{ "studentId": 3, "trainingTrackId": 1 }
# response Data.Status == "Pending"

# Track capacity ignores cancelled enrollments
PUT /api/enrollments/{id}/status  { "status": "Cancelled" }
# then re-enroll another student into the same track and confirm capacity isn't blocked by the cancelled slot
```

### Payment
```http
# Amount 0 -> 400
POST /api/payments
{ "enrollmentId": 1, "amount": 0, "paymentMethod": "CreditCard" }

# Amount above remaining -> 400
POST /api/payments
{ "enrollmentId": 1, "amount": 999999, "paymentMethod": "CreditCard" }

# Failed payment not counted in revenue
PUT /api/payments/{id}/status  { "status": "Failed" }
GET /api/reports/revenue-summary
# TotalRevenue/PaidAmount must not include the Failed payment's amount
```

---

## 🖼 Screenshots

Swagger evidence for each rule — failed (400) request and successful valid request per rule — go here.

> 📌 **[View Screenshots on Google Drive](https://drive.google.com/drive/folders/1ziL4hv7rHyv_fv981d3EoVSZfOgBe6LQ?usp=drive_link)**
