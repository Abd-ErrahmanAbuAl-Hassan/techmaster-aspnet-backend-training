# 🛡️ Phase 04 — Task 02: Role Stories & Access Control

Generic CRUD endpoints turned into workflows that respect **Admin**, **Instructor**, and **Student** boundaries. Two questions are answered for every protected endpoint:

1. **Role check** — may this *kind* of user call it at all? (`[Authorize(Roles = "...")]`)
2. **Ownership check** — may *this* user touch *this* record? (e.g. an instructor reading another instructor's track, a student reading another student's profile)

---

## 📖 Table of Contents

- [Role Stories](#-role-stories)
- [Access Matrix](#-access-matrix)
- [What Happens When the Wrong Caller Tries](#-what-happens-when-the-wrong-caller-tries)
- [Endpoint Reference](#-endpoint-reference)
- [Required Authorization Tests](#-required-authorization-tests)
- [Evidence / Screenshots](#-evidence--screenshots)

---

## 👥 Role Stories

| Role | Can | Cannot |
|------|-----|--------|
| **Admin** | Manage students, instructors and tracks; view all enrollments; create payments and update payment status; access all reports | — |
| **Instructor** | View own profile and own assigned tracks; view students in own tracks; update own tracks (limited) | Create instructors; see payment revenue or reports; touch another instructor's track |
| **Student** | View own profile, own enrollments, own payments; view available tracks; enroll themselves | See other students; enroll another student; see admin reports; change payment status |

---

## 🧭 Access Matrix

The source of truth for authorization testing.

| Endpoint group | Admin | Instructor | Student | Notes |
|----------------|-------|------------|---------|-------|
| Students CRUD | Full access | Reads students **in own tracks only** — via `GET /api/tracks/{id}/students` | Own profile only | No public student list |
| Instructors CRUD | Full access | Own profile only | No access | An instructor cannot create another instructor |
| Tracks | Full access | Assigned tracks, read/update only | Read **available** (published) tracks | Track management is admin-owned |
| Enrollments | Full access | Reads enrollments of own tracks | Own enrollments only | A student cannot enroll another student |
| Payments | Full access | **No revenue access** | Own payment history only | Payment status and payment creation are admin-only |
| Reports | Full admin reports | Own track reports — **not implemented yet** | No admin reports | Role-specific reporting |
| Audit logs | Full access | Own operations (optional) | No access | **Not implemented yet** — audit trail is a later Phase 04 task |

---

## 🚪 What Happens When the Wrong Caller Tries

| Situation | Response | Where it comes from |
|-----------|----------|---------------------|
| No token, expired token, or invalid signature | **`401 Unauthorized`** (empty body) | JWT authentication middleware |
| Valid token, but the endpoint's roles don't include the caller's role | **`403 Forbidden`** (empty body) | Authorization middleware (`[Authorize(Roles = ...)]`) |
| Right role, but the record isn't theirs (another student's profile, another instructor's track…) | **`403 Forbidden`** with a JSON body: `{ "success": false, "message": "Access denied.", "statusCode": 403, "errors": ["…"] }` | Ownership guard in the controller |
| Record doesn't exist | **`404 Not Found`** | Service result |

The caller's identity **always comes from the token** (`NameIdentifier` claim) — never from a request body or query string.

---

## 📡 Endpoint Reference

Every protected endpoint: who can call it, why, and what a wrong caller gets. "Other roles" below always means `403` by the rules above, and a missing/invalid token always means `401`.

### Auth — `/api/auth` *(Task 01)*

| Endpoint | Who | Why |
|----------|-----|-----|
| `POST /register`, `POST /login`, `POST /refresh-token` | Anonymous | Callers don't have a token yet |
| `GET /me`, `POST /change-password`, `POST /logout` | Any authenticated user | Operate on the caller's own account |

### Students — `/api/students`

| Endpoint | Who | Why | Wrong caller |
|----------|-----|-----|--------------|
| `GET /api/students` | Admin | There is no public student list | Student / Instructor → `403` |
| `GET /api/students/me` | Student | A student's own profile | Admin / Instructor → `403` |
| `GET /api/students/my-enrollments` (also `/api/student/my-enrollments`) | Student | A student's own enrollments | Admin / Instructor → `403` |
| `GET /api/students/{id}` | Admin; Student **only for their own id** | Students see only themselves. Instructors are excluded because a full profile exposes the student's other tracks and payment status | Student with another id → `403` (JSON); Instructor → `403` |
| `POST /api/students` | Admin | Self-service sign-up is `POST /api/auth/register` | Student / Instructor → `403` |
| `PUT /api/students/{id}` | Admin; Student **only for their own id** | Students edit their own profile; only admins change `isActive` (it's ignored for students) | Student with another id → `403` (JSON) |
| `DELETE /api/students/{id}` | Admin; Student **only for their own id** | Soft-delete of one's own account | Student with another id → `403` (JSON) |

### Instructors — `/api/instructors`

| Endpoint | Who | Why | Wrong caller |
|----------|-----|-----|--------------|
| `GET /api/instructors` | Admin | Full instructor list is admin data | Instructor / Student → `403` |
| `GET /api/instructors/my-tracks` (also `/api/instructor/my-tracks`) | Instructor | The tracks assigned to them | Admin / Student → `403` |
| `GET /api/instructors/{id}` | Admin; Instructor **only for their own id** | Instructors see only their own profile | Instructor with another id → `403` (JSON); Student → `403` |
| `GET /api/instructors/{id}/tracks` | Admin; Instructor **only for their own id** | Same | Same |
| `POST /api/instructors` | Admin | An instructor cannot create another instructor | Instructor / Student → `403` |
| `PUT /api/instructors/{id}` | Admin; Instructor **only for their own id** | Own profile only; `isActive` is admin-only (ignored for instructors) | Instructor with another id → `403` (JSON) |

### Tracks — `/api/tracks`

| Endpoint | Who | Why | Wrong caller |
|----------|-----|-----|--------------|
| `GET /api/tracks` | Admin | Full catalogue including unpublished/closed tracks | Instructor / Student → `403` |
| `GET /api/tracks/available` | Admin, Instructor, Student | The student-facing catalogue — published tracks only | — |
| `GET /api/tracks/{id}` | Admin (any); Instructor (**assigned** only); Student (**published** only) | Instructors see their own tracks; students see only what's available | Instructor, other's track → `403` (JSON); Student, unpublished track → `404` |
| `POST /api/tracks` | Admin | Track management is admin-owned | Instructor / Student → `403` |
| `PUT /api/tracks/{id}` | Admin; Instructor **assigned to that track** | Limited update of own tracks | Other instructor → `403` (JSON); Student → `403` |
| `DELETE /api/tracks/{id}` | Admin | Track management is admin-owned | Instructor / Student → `403` |
| `GET /api/tracks/{id}/students` | Admin; Instructor **assigned to that track** | Instructors see the students of their own tracks | Other instructor → `403` (JSON); Student → `403` |

### Enrollments — `/api/enrollments`

| Endpoint | Who | Why | Wrong caller |
|----------|-----|-----|--------------|
| `GET /api/enrollments` | Admin | All enrollments, with student details and payments | Instructor / Student → `403` |
| `GET /api/enrollments/{id}` | Admin; Student **only if it's theirs** | The detail includes payments, so instructors are excluded | Other student's enrollment → `403` (JSON); Instructor → `403` |
| `POST /api/enrollments` | Admin; Student **only with their own `studentId`** | A student cannot enroll another student. `allowInactiveStudent` is honoured for admins only | Student with another `studentId` → `403` (JSON); Instructor → `403` |
| `PUT /api/enrollments/{id}/status` | Admin | Activating, completing or cancelling is an administrative decision | Instructor / Student → `403` |
| `GET /api/enrollments/student/{studentId}` | Admin; Student **only for their own id** | Own enrollment history | Student with another id → `403` (JSON); Instructor → `403` |
| `GET /api/enrollments/track/{trackId}/students` | Admin; Instructor **assigned to that track** | Same rule as `GET /api/tracks/{id}/students` | Other instructor → `403` (JSON); Student → `403` |

### Payments — `/api/payments`

| Endpoint | Who | Why | Wrong caller |
|----------|-----|-----|--------------|
| `GET /api/payments` | Admin | All payments are revenue data | Instructor / Student → `403` |
| `GET /api/payments/my-payments` | Student | Own payment history across their enrollments (latest 50 enrollments) | Admin / Instructor → `403` |
| `POST /api/payments` | Admin | Recording a payment marks it Paid and activates the enrollment — a student able to call it could mark their own enrollment paid | Instructor / Student → `403` |
| `GET /api/payments/enrollment/{enrollmentId}` | Admin; Student **only for their own enrollment** | Own payment history only; instructors have no revenue access | Other student's enrollment → `403` (JSON); Instructor → `403` |
| `PUT /api/payments/{id}/status` | Admin | Payment status is admin-only | Instructor / Student → `403` |

### Reports — `/api/reports`

| Endpoint | Who | Why | Wrong caller |
|----------|-----|-----|--------------|
| All 9 report endpoints (`dashboard-summary`, `revenue-summary`, `revenue-by-track`, `unpaid-enrollments`, `track-capacity`, `tracks-with-available-seats`, `top-tracks`, `instructors-workload`, `students-without-payments`) | Admin | Admin reporting includes revenue; instructors have no revenue access | Instructor / Student → `403` |

---

## 🧪 Required Authorization Tests

At least 8 Postman requests are required as evidence. Screenshots must show the token's role (or a collection variable such as `{{studentToken}}`).

| # | Test | Request | Expected |
|---|------|---------|----------|
| 1 | No token | `GET /api/reports/revenue-summary` | `401` |
| 2 | Student token on admin report | `GET /api/reports/revenue-summary` | `403` |
| 3 | Instructor sees own tracks | `GET /api/instructor/my-tracks`  | `200` |
| 4 | Instructor, another instructor's track | `GET /api/tracks/{otherTrackId}/students` | `403` |
| 5 | Student sees own enrollments | `GET /api/student/my-enrollments`  | `200` |
| 6 | Student, another student's profile | `GET /api/students/{otherStudentId}` | `403` |
| 7 | Admin updates payment status | `PUT /api/payments/{id}/status` | `200` |
| 8 | Student updates payment status | `PUT /api/payments/{id}/status` | `403` |

---


## 🖼 Evidence / Screenshots

> 📌 **[View Evidence on Google Drive](https://drive.google.com/drive/folders/10TP92vSUz-_Jb8W10hHIOYU6zTIHL7N_?usp=drive_link)**

