# 🛡️ Phase 04 — Task 05: Validation, Errors & Logging

The Training Center API protects itself with **layered validation**, a **single error strategy** (expected failures are returned as a `Result`, unexpected ones hit a global exception handler), and **logging** that never records secrets.

This README documents what the code does today. Anything that isn't finished yet is stated plainly in [Open Items](#-open-items-against-the-task-05-spec) rather than implied.

---

## 📖 Table of Contents

- [Validation Strategy](#-validation-strategy)
- [Validation & Business Rule Bank](#-validation--business-rule-bank)
- [Error Strategy](#-error-strategy)
- [Global Exception Handler](#-global-exception-handler)
- [Error Handling Test Cases](#-error-handling-test-cases)
- [Logging](#-logging)
- [Open Items Against the Task 05 Spec](#-open-items-against-the-task-05-spec)
- [Evidence](#-evidence)

---

## ✅ Validation Strategy

A request passes through four checkpoints, from cheapest to most specific. Each rule lives in the layer that owns it.

| # | Checkpoint | Where | What it catches |
|---|-----------|-------|-----------------|
| 1 | **Model binding** | `[ApiController]` + DataAnnotations on request DTOs; `ModelState` checks in `AuthController` | Malformed JSON, missing required properties |
| 2 | **Access control** | `[Authorize(Roles = …)]` and the ownership guards in `API/Extensions/AccessControlExtensions` | Wrong role (`403`), not the owner of the record (`403`), no token (`401`) |
| 3 | **Request validators** | Static classes in `Application/Validations` (`AuthValidation`, `UserValidation`, `TrackValidation`) | Required fields, formats, URL and date checks — returned as a readable list of messages |
| 4 | **Business rules** | Services in `Application/Services/Implementations` | Uniqueness, capacity, duplicates, state transitions, payment limits, existence of referenced records |

Controllers also reject non-positive ids and invalid paging (`page ≥ 1`, `pageSize` 1–50) before a service is called.

---

## 📋 Validation & Business Rule Bank

**38 rules**, grouped by feature. *Type* uses the task's categories: **V**alidation, **B**usiness rule, **R**ole, **O**wnership, **D**ate/**A**mount.

### Register / Auth

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 1 | First name and last name are required | V | `AuthValidation.RegisterValidate` |
| 2 | Email is required and must have a valid format | V | `AuthValidation` (register and login) |
| 3 | Phone number is required and must be an Egyptian mobile number (`01[0125]XXXXXXXX`) | V | `AuthValidation` |
| 4 | Password and confirm-password are required and must match | V | `AuthValidation.RegisterValidate` |
| 5 | Email must be unique | B | `AuthService.RegisterAsync` |
| 6 | The role must be a defined value | V | `AuthService.RegisterAsync` |
| 7 | **Admin cannot be self-registered** — only Student or Instructor | R | `AuthController.Register` |
| 8 | Inactive users cannot log in | B | `AuthService.LoginAsync` |
| 9 | Wrong email and wrong password return the **same** generic `401` (no account enumeration) | B | `AuthService.LoginAsync` |
| 10 | Change password: old password required and verified, new ≠ old, confirmation must match | V/B | `AuthService.ChangePasswordAsync` |
| 11 | A refresh token must exist, be unexpired, and not be revoked | B | `AuthService.RefreshTokenAsync` |

### Tracks

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 12 | Title and description are required | V | `TrainingTrackService.CreateTrackAsync` |
| 13 | Capacity must be greater than zero (create **and** update) | V | `TrainingTrackService` |
| 14 | Price must be greater than zero (create and update) | D/A | `TrainingTrackService` |
| 15 | Start date must be before end date (update requires both dates or neither) | D/A | `TrainingTrackService` |
| 16 | The instructor must exist | B | `TrainingTrackService.CreateTrackAsync` |
| 17 | Track level must be a defined value | V | `TrainingTrackService` |
| 18 | Capacity cannot be lowered below the number of currently enrolled students | B | `TrainingTrackService.UpdateTrackAsync` |
| 19 | Track code must be unique (regenerated on collision) | B | `TrainingTrackService.CreateTrackAsync` |
| 20 | A track with active enrollments cannot be deleted | B | `TrainingTrackService.DeleteTrackAsync` |
| 21 | Only an admin may create or delete tracks; an instructor may update **only a track assigned to them** | R/O | `TracksController` + `EnsureTrackAccessAsync` |
| 22 | Assigning an instructor: the instructor must exist, be active, and not already be assigned | B | `TrainingTrackService.AssignInstructorToTrackAsync` |

### Enrollments

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 23 | The student and the track must exist | B | `EnrollmentService.CreateEnrollmentAsync` |
| 24 | The track must be open — a closed track accepts no enrollments | B | `EnrollmentService.CreateEnrollmentAsync` |
| 25 | An inactive or deleted student cannot be enrolled (admin-only override) | B/R | `EnrollmentService` + `EnrollmentsController` |
| 26 | No duplicate enrollment — a student can't have two non-cancelled enrollments in the same track | B | `EnrollmentService.CreateEnrollmentAsync` |
| 27 | Track capacity must not be exceeded ⚠️ *see [open items](#-open-items-against-the-task-05-spec)* | B | `EnrollmentService.CreateEnrollmentAsync` |
| 28 | Status changes follow an allowed-transition table (anything else is rejected) | B | `EnrollmentService.IsValidStatusTransition` |
| 29 | A student can enroll **only themselves**; only an admin can change an enrollment's status | O/R | `EnrollmentsController` |

### Payments

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 30 | Amount must be greater than zero | D/A | `PaymentService.CreatePaymentAsync` |
| 31 | The payment method must be a defined value | V | `PaymentService.CreatePaymentAsync` |
| 32 | The enrollment must exist and must not be cancelled | B | `PaymentService.CreatePaymentAsync` |
| 33 | An enrollment that is already fully paid cannot be paid again | B | `PaymentService.CreatePaymentAsync` |
| 34 | The amount cannot exceed the remaining balance (overpayment is rejected) | D/A | `PaymentService.CreatePaymentAsync` |
| 35 | Payment status must be a defined value and changes follow an allowed-transition table | V/B | `PaymentService.UpdatePaymentStatusAsync` |
| 36 | Date-range filter: start and end dates must be given together, start ≤ end | D/A | `PaymentService.GetPaymentsAsync` |
| 37 | Only an admin can create payments or change payment status; a student sees only payments of their own enrollment | R/O | `PaymentsController` |

### Sessions

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 38 | Title, description and meeting link are required; the link must be an absolute `http`/`https` URL with a host | V | `TrackValidation.ValidateTrackSession` |
| 39 | The session date cannot be in the past | D/A | `TrackValidation.ValidateTrackSession` |
| 40 | The track must belong to the instructor creating or updating the session | O | `TrainingTrackService.CreateTrackSessionAsync` / `UpdateTrackSessionAsync` |
| 41 | The track and the instructor must both be active | B | `TrainingTrackService` (sessions) |

### Profile & Ownership

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 42 | A student can update only allowed fields: `UpdateStudentRequest` has **no role property**, and `isActive` is ignored for non-admins | V/R | `UpdateStudentRequest` + `StudentsController.UpdateStudent` |
| 43 | A student can read, update or delete **only their own** profile (another student's id → `403`) | O | `StudentsController` |
| 44 | Student and instructor email and phone must be unique (create and update) | B | `StudentService`, `InstructorService` |
| 45 | Paging must be valid: `pageNumber ≥ 1`, `pageSize` between 1 and 50 | V | Services + controllers |

---

## 🚨 Error Strategy

**Principle:** *expected* problems are values, *unexpected* problems are exceptions.

| Kind of failure | How it is handled | Client sees |
|-----------------|-------------------|-------------|
| **Expected** — validation, business-rule, not found, conflict | The service returns `Result.FailureResult(message, errors, statusCode)`. No exception is thrown for control flow | The standard JSON body and the matching HTTP status |
| **Result → HTTP** | One helper, `FailureResponse`, maps `400 / 401 / 403 / 404 / 409` and falls back to `500` | — |
| **No / invalid / expired token** | JWT authentication middleware | `401` |
| **Wrong role** | `[Authorize(Roles = …)]` | `403` |
| **Not the owner of the record** | Ownership guards (`AccessControlExtensions`) | `403` with `{ "success": false, "message": "Access denied.", … }` |
| **Malformed body / failed model binding** | `[ApiController]` automatic validation | `400` |
| **Unexpected exception** | The global exception handler (below) | Generic JSON `500` |

**The standard body** (same wrapper for success and failure):

```json
{
  "success": false,
  "message": "Validation errors",
  "errors": [ "Email is required.", "Password and confirm password do not match." ],
  "statusCode": 400
}
```

**Safe by design:** the global handler never returns a stack trace or `ToString()` of an exception; the exception message is included **only in Development**; login failures return one generic message regardless of whether the email or the password was wrong.

---

## 🧱 Global Exception Handler

Registered in `TrainingCenter.API/Program.cs`, ahead of the rest of the pipeline, using ASP.NET Core's built-in `UseExceptionHandler`:

```csharp
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        logger.LogError(exception, "Unhandled exception for {Method} {Path}",
            context.Request.Method, context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new
        {
            Success = false,
            Message = "An unexpected error occurred.",
            ErrorCode = 500,
            Errors = app.Environment.IsDevelopment() && exception != null
                ? new List<string> { exception.Message }
                : new List<string>()
        };

        // …serialized as camelCase JSON and written to the response
    });
});
```

What it does against the spec's checklist:

| Middleware must… | |
|------------------|---|
| Catch unhandled exceptions | ✅ |
| Log the exception internally | ✅ `LogError` with the exception, HTTP method and path |
| Return a safe API response | ✅ generic message, empty `errors` outside Development |
| Use `500` for unexpected errors | ✅ |
| Never expose a raw stack trace in production | ✅ only `exception.Message`, and only in Development |
| Not log sensitive user secrets | ✅ it logs the exception, method and path only — never the request body, headers or tokens |

---

## 🧪 Error Handling Test Cases

| Case | Expected | Status in this codebase |
|------|----------|--------------------------|
| Missing resource id | `404` + not-found message | ✅ Auth, tracks, sessions. ⚠️ Students, instructors, enrollments and payments currently return `500` — see open items |
| Invalid request body | `400` + validation errors | ✅ Model-binding failures and the Auth/Track/Session validators. ⚠️ Validators in the Student/Instructor/Enrollment/Payment services currently return `500` |
| No token | `401` | ✅ (empty body — see open items) |
| Wrong role | `403` | ✅ role mismatch returns an empty `403`; ownership mismatch returns the JSON `"Access denied."` body |
| Unexpected exception | `500` + safe generic message | ✅ for unhandled exceptions. ⚠️ exceptions caught inside services are returned as `500` with the raw `ex.Message` |

---

## 📝 Logging

**Configuration:** console and debug providers; default level `Information`, with `Microsoft.AspNetCore` at `Warning`. Entity Framework SQL command logging is on in Development only, and sensitive-data logging is **not** enabled, so parameter values are masked.

**What is logged today**

| Event | Level | Where |
|-------|-------|-------|
| Unhandled exception, with HTTP method and path | Error | Global exception handler |
| Database migrations: start, success | Information | `Program.ApplyMigrationsAsync` |
| Database migration failure | Critical | `Program.ApplyMigrationsAsync` |

**Required events from the task**

| Required event | Status |
|----------------|--------|
| Log exceptions | ⚠️ Unhandled ones are logged; exceptions caught inside services are not |
| Log login success / failure | ❌ Not yet — `AuthService` has no logger |
| Log payment updates | ❌ Not yet |
| Log enrollment changes | ❌ Not yet |
| Never log passwords or tokens | ✅ Nothing in the code logs credentials, tokens, request bodies or headers |

---

## 🚧 Open Items Against the Task 05 Spec

Listed so the status above is verifiable, and so each can be closed and removed from this list:

1. **Status codes in several services.** `Result.FailureResult` defaults to `500`, and the Student, Instructor, Enrollment, Payment and Report services mostly don't pass a status code — so their validation and not-found failures return `500`. The spec says validation errors must **not** be hidden as `500`.
2. **Repeated `try/catch`.** About 35 service methods wrap their body in `try/catch` and return `ex.Message` to the client — the opposite of centralizing unexpected errors, and it bypasses both the global handler and logging.
3. **Logging of business events** (login, payments, enrollments) is not implemented.
4. **`401` / `403` bodies** from the framework are empty; the spec expects *"Authentication required"* and *"Access denied"* messages.
5. **The handler is inline in `Program.cs`** rather than a dedicated middleware class, and its JSON uses `errorCode` where the `Result` wrapper uses `statusCode`.
6. **Register has no password-strength rule** (only required + must match); the spec asks for a strong password.
7. **Model-binding `400`s** use ASP.NET's default validation-problem format, which differs from the `Result` body.
8. **Rule 27 (capacity)** — the enrolled-count query combines its conditions without parentheses, so it counts more than the target track's enrollments.

---

## 🖼 Evidence

> 📌 **[View Evidence on Google Drive](https://drive.google.com/your-evidence-link-here)**

| Required evidence | Preview |
|-------------------|---------|
| Screenshot — validation error (e.g. `POST /api/auth/register` with a bad email and mismatched passwords → `400`) | *(placeholder)* |
| Screenshot — unauthorized request (`GET /api/auth/me` with no token → `401`) | *(placeholder)* |
| Screenshot — forbidden request (student token on `GET /api/reports/revenue-summary` → `403`) | *(placeholder)* |
| Middleware code snippet | ✅ included in [Global Exception Handler](#-global-exception-handler) |
| Error-strategy explanation | ✅ [Error Strategy](#-error-strategy) |