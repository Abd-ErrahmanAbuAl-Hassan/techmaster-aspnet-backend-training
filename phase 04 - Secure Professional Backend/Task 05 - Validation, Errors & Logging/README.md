# 🛡️ Phase 04 — Task 05: Validation, Errors & Logging

The Training Center API protects itself with **layered validation**, a **single error strategy** (expected failures are returned as a typed `Result`; unexpected ones reach a global exception middleware), and **structured logging** of security- and business-relevant events that never records passwords or tokens.

This README documents what the code does today. Anything not finished is stated plainly in [Open Items](#-open-items-against-the-task-05-spec) rather than implied.

---

## 📖 Table of Contents

- [Validation Strategy](#-validation-strategy)
- [Validation & Business Rule Bank](#-validation--business-rule-bank)
- [Error Strategy](#-error-strategy)
- [Global Exception Middleware](#-global-exception-middleware)
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
| 2 | **Access control** | `[Authorize(Roles = …)]` and the ownership guards in `API/Extensions/AccessControlExtensions` | No token (`401`), wrong role (`403`), not the owner of the record (`403`) |
| 3 | **Request validators** | Static classes in `Application/Validations` (`AuthValidation`, `UserValidation`, `TrackValidation`) | Required fields, formats, URL and date checks — returned as a readable list |
| 4 | **Business rules** | Services in `Application/Services/Implementations` | Uniqueness, capacity, duplicates, state transitions, payment limits, existence of referenced records |

Controllers also reject non-positive ids and invalid paging (`pageNumber ≥ 1`, `pageSize` 1–50) before a service is called. All user-facing messages come from one catalog, **`Application/Helpers/ResultMessages`** (`Success`, `Validation`, `NotFound`, `Conflict`, `Unauthorized`, `ServerError`), so the same situation always produces the same wording.

---

## 📋 Validation & Business Rule Bank

**49 rules**, grouped by feature. *Type* uses the task's categories: **V**alidation, **B**usiness rule, **R**ole, **O**wnership, **D**ate/**A**mount.

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
| 11 | A refresh token must exist, be unexpired, not be revoked, and belong to an active user | B | `AuthService.RefreshTokenAsync` |
| 12 | Logout requires a refresh token that belongs to the caller | O | `AuthService.LogoutAsync` |

### Tracks

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 13 | Title and description are required | V | `TrainingTrackService.CreateTrackAsync` |
| 14 | Capacity must be greater than zero (create **and** update) | V | `TrainingTrackService` |
| 15 | Price must be greater than zero (create and update) | D/A | `TrainingTrackService` |
| 16 | Start date must be before end date (update requires both dates or neither) | D/A | `TrainingTrackService` |
| 17 | The instructor must exist | B | `TrainingTrackService.CreateTrackAsync` |
| 18 | Track level must be a defined value | V | `TrainingTrackService` |
| 19 | Capacity cannot be lowered below the number of currently enrolled students | B | `TrainingTrackService.UpdateTrackAsync` |
| 20 | Track code must be unique (regenerated on collision) | B | `TrainingTrackService.CreateTrackAsync` |
| 21 | A track with active enrollments cannot be deleted | B | `TrainingTrackService.DeleteTrackAsync` |
| 22 | Only an admin may create or delete tracks; an instructor may update **only a track assigned to them** | R/O | `TracksController` + `EnsureTrackAccessAsync` |
| 23 | Assigning an instructor: the instructor must exist, be active, and not already be assigned | B | `TrainingTrackService.AssignInstructorToTrackAsync` |

### Enrollments

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 24 | The student and the track must exist | B | `EnrollmentService.CreateEnrollmentAsync` |
| 25 | The track must be open — a closed track accepts no enrollments | B | `EnrollmentService.CreateEnrollmentAsync` |
| 26 | An inactive or deleted student cannot be enrolled (admin-only override) | B/R | `EnrollmentService` + `EnrollmentsController` |
| 27 | No duplicate enrollment — a student can't have two non-cancelled enrollments in the same track | B | `EnrollmentService.CreateEnrollmentAsync` |
| 28 | Track capacity must not be exceeded (active + completed enrollments of **that track** are counted) | B | `EnrollmentService.CreateEnrollmentAsync` |
| 29 | Status changes follow an allowed-transition table; anything else is rejected | B | `EnrollmentService.IsValidStatusTransition` |
| 30 | A student can enroll **only themselves**; only an admin can change an enrollment's status | O/R | `EnrollmentsController` |

### Payments

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 31 | Amount must be greater than zero | D/A | `PaymentService.CreatePaymentAsync` |
| 32 | The payment method must be a defined value | V | `PaymentService.CreatePaymentAsync` |
| 33 | The enrollment must exist | B | `PaymentService.CreatePaymentAsync` |
| 34 | The payer must own the enrollment (`enrollment.StudentId == request.UserId`), otherwise `403` | O | `PaymentService.CreatePaymentAsync` |
| 35 | A cancelled enrollment cannot be paid | B | `PaymentService.CreatePaymentAsync` |
| 36 | An enrollment that is already fully paid cannot be paid again | B | `PaymentService.CreatePaymentAsync` |
| 37 | The amount cannot exceed the remaining balance (overpayment is rejected) | D/A | `PaymentService.CreatePaymentAsync` |
| 38 | Payment status must be a defined value and changes follow an allowed-transition table | V/B | `PaymentService.UpdatePaymentStatusAsync` |
| 39 | Date-range filter: start and end dates must be given together, start ≤ end | D/A | `PaymentService.GetPaymentsAsync` |
| 40 | Only an admin can change payment status; a student sees only payments of their own enrollment | R/O | `PaymentsController` |
| 41 | The payment and the resulting enrollment activation commit **atomically** (one transaction, wrapped in the EF execution strategy) | B | `PaymentService.CreatePaymentAsync` |

### Sessions

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 42 | Title, description and meeting link are required; the link must be an absolute `http`/`https` URL with a host | V | `TrackValidation.ValidateTrackSession` |
| 43 | The session date cannot be in the past | D/A | `TrackValidation.ValidateTrackSession` |
| 44 | The track must belong to the instructor creating or updating the session | O | `TrainingTrackService.CreateTrackSessionAsync` / `UpdateTrackSessionAsync` |
| 45 | The track and the instructor must both be active | B | `TrainingTrackService` (sessions) |

### Profile & Ownership

| # | Rule | Type | Enforced in |
|---|------|------|-------------|
| 46 | A student can update only allowed fields: `UpdateStudentRequest` has **no role property**, and `isActive` is ignored for non-admins | V/R | `UpdateStudentRequest` + `StudentsController.UpdateStudent` |
| 47 | A student can read, update or delete **only their own** profile (another student's id → `403`) | O | `StudentsController` |
| 48 | Student and instructor email and phone must be unique (create and update) | B | `StudentService`, `InstructorService` |
| 49 | Paging must be valid: `pageNumber ≥ 1`, `pageSize` between 1 and 50 | V | Services + controllers |

---

## 🚨 Error Strategy

**Principle:** *expected* problems are values, *unexpected* problems are exceptions.

| Kind of failure | How it is handled | Client sees |
|-----------------|-------------------|-------------|
| **Expected** — validation, business rule, not found, conflict, auth | The service returns a typed result: `ValidationErrorResult`, `NotFoundResult`, `ConflictResult`, `UnauthorizedResult` or `ForbiddenResult`, with a message from `ResultMessages`. No exception is used for control flow | The standard JSON body and the matching HTTP status |
| **Result → HTTP** | One helper, `FailureResponse`, maps `400 / 401 / 403 / 404 / 409` and falls back to `500` | — |
| **No / invalid / expired token** | JWT authentication middleware | `401` |
| **Wrong role** | `[Authorize(Roles = …)]` | `403` |
| **Not the owner of the record** | Ownership guards (`AccessControlExtensions`) or the service (`ForbiddenResult`) | `403` |
| **Malformed body / failed model binding** | `[ApiController]` automatic validation | `400` |
| **Unexpected exception** | Logged by the service that caught it (`LogError`); anything that escapes reaches the global middleware | `500` (or the mapped status — see below) |

**The standard body** (same wrapper for success and failure):

```json
{
  "success": false,
  "message": "Validation errors occurred. Please review the details below.",
  "errors": [ "Email is required.", "Password and confirm password do not match." ],
  "statusCode": 400
}
```

**What is never returned:** a stack trace, or `exception.ToString()`. Login failures return one generic message whether the email or the password was wrong.

---

## 🧱 Global Exception Middleware

`TrainingCenter.API/Middleware/GlobalExceptionHandlingMiddleware.cs`, registered in `Program.cs` after Swagger and **before** HTTPS redirection, authentication and routing — so it wraps everything that follows:

```csharp
// Program.cs
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

```csharp
public async Task InvokeAsync(HttpContext context)
{
    try
    {
        await _next(context);
    }
    catch (Exception exception)
    {
        await HandleExceptionAsync(context, exception);
    }
}

private Task HandleExceptionAsync(HttpContext context, Exception exception)
{
    _logger.LogError(exception, "Unhandled exception occurred for {Method} {Path}. ...",
        context.Request.Method, context.Request.Path, exception);

    context.Response.ContentType = "application/json";
    var (statusCode, message, errors) = MapExceptionToResponse(exception);
    context.Response.StatusCode = statusCode;

    var response = new { success = false, message, statusCode, errors, timestamp = DateTime.UtcNow };
    // …serialized as camelCase JSON and written to the response
}
```

**Exception → response mapping**

| Exception | Status | Message |
|-----------|--------|---------|
| `ArgumentNullException` | `400` | Invalid request: a required argument was null |
| `ArgumentException` | `400` | Invalid request: an argument validation failed |
| `InvalidOperationException` | `400` | The requested operation cannot be completed in the current state |
| `KeyNotFoundException` | `404` | The requested resource was not found |
| `UnauthorizedAccessException` | `403` | Access denied |
| `TimeoutException` | `408` | The request took too long to complete |
| `DbUpdateConcurrencyException` | `409` | Concurrency conflict: the record was modified by another user |
| `DbUpdateException` | `409` | A database error occurred |
| anything else | `500` | An unexpected server error occurred |

**Against the spec's checklist**

| Middleware must… | |
|------------------|---|
| Catch unhandled exceptions | ✅ |
| Log the exception internally | ✅ `LogError` with the exception, HTTP method and path |
| Return an API response in the standard shape | ✅ `success`, `message`, `statusCode`, `errors` (+ `timestamp`) |
| Use `500` for unexpected errors | ✅ the catch-all arm; known exception types map to more specific codes |
| Never expose a raw stack trace | ✅ stack traces are never written to the response |
| Not log sensitive user secrets | ✅ it logs the exception, method and path — never request bodies, headers or tokens |

---

## 🧪 Error Handling Test Cases

| Case | Expected | Status in this codebase |
|------|----------|--------------------------|
| Missing resource id | `404` + not-found message | ✅ Students, instructors, enrollments, payments, sessions, reports, auth (via `NotFoundResult`).  |
| Invalid request body | `400` + validation errors | ✅ Model-binding failures and every migrated service (`ValidationErrorResult`).  |
| No token | `401` | ✅ (empty body — see open items) |
| Wrong role | `403` | ✅ role mismatch returns an empty `403`; ownership mismatch returns the JSON body |

---

## 📝 Logging

**Configuration:** console and debug providers, minimum level `Information`; `ILogger<T>` is injected into every service. Messages are **structured** (named placeholders such as `{UserId}`, `{EnrollmentId}`, `{PaymentId}`), so they can be filtered and queried instead of searched as text.

**Levels:** `Information` for normal events and successes, `Warning` for rejected requests (validation failures, ownership violations, invalid transitions, failed logins), `Error` for exceptions, `Critical` for a failed database migration.

**Required events from the task**

| Required event | Status | What is logged |
|----------------|--------|----------------|
| **Login success / failure** | ✅ | Attempt (email); success (user id, email); failure with the reason — validation failed, user not found, user inactive, invalid password |
| **Payment updates** | ✅ | Payment creation (enrollment id, amount, user id), validation / ownership / already-paid / overpayment rejections, enrollment activated by full payment, success (payment id); payment **status change** requests, invalid transitions, success |
| **Enrollment changes** | ✅ | Enrollment created (id, student, track) and every rejection reason; status-change request, invalid transition, success; refund processing for cancelled enrollments |
| **Exceptions** | ✅ | Every service method logs the caught exception with `LogError`; the global middleware logs anything unhandled with the request method and path |
| **No passwords or tokens in logs** | ✅ | No log call has a password, token, password-hash or refresh-token placeholder, and no request body is logged |

**Also logged:** registration (attempt, rejection reasons, success), password change, token refresh (each failure reason), logout, student/instructor creation, and database migrations.

**What identifiers are logged:** user ids, entity ids, and the email address on authentication events; student/instructor creation logs email and phone number. Treat these logs as containing personal data.

---

## 🚧 Open Items Against the Task 05 Spec

Listed so the status above is verifiable, and so each can be closed and removed from this list:

1. **Two track methods are not migrated.** `TrainingTrackService.GetTrackByIdAsync` and `CreateTrackAsync` still use the older failure style (no typed result factories, no logging), unlike every other service method.
2. **Exception messages reach the client.** The middleware puts `exception.Message` into `errors` in every environment (and the inner message for `DbUpdateException`), and each service's `catch` block returns `ex.Message` in its `Result`. The spec asks for a safe generic message.
3. **Every service method still has its own `try/catch`.** The spec asks to centralize unexpected errors instead of repeating `try/catch`; because services catch first, most exceptions never reach the middleware.
4. **Broad exception mapping.** `InvalidOperationException` and `ArgumentException` map to `400`, and `TimeoutException` to `408`; those types are also raised by genuine server faults, which would then be reported as client errors.
5. **`401` / `403` responses from the framework have an empty body.** The spec expects *"Authentication required"* and *"Access denied"* messages.
6. **Register has no password-strength rule** in the code reviewed — only required + must match. `ResultMessages.Validation.PasswordWeak` exists, but nothing uses it.
7. **Model-binding `400`s** use ASP.NET's default validation-problem format, which differs from the standard body.

---

## 🖼 Evidence

> 📌 **[View Evidence on Google Drive](https://drive.google.com/drive/folders/1XNKC36uIjZQfDhqIl1zAt_SgLVoo31X2?usp=drive_link)**
