# 🚀 Phase 04 — Task 03: Secure Platform Upgrade

The Phase 03 Training Center API upgraded into a **role-based platform**: anonymous access removed from sensitive endpoints, role checks and ownership checks applied across every controller, and the user stories for **Admin**, **Instructor**, and **Student** mapped to real endpoints.

> **Main rule:** if an endpoint handles private data, payment data, admin data, or another user's identity, it must be protected.

The full per-endpoint access matrix (who can call it, why, and what a wrong caller gets) is in the **Task 02 README**. This README covers what Task 03 adds on top: story coverage, the new endpoints, deployment, and what's still open.

---

## 📖 Table of Contents

- [Upgrade Areas](#-upgrade-areas)
- [Ownership Rule](#-ownership-rule)
---

## 🧱 Upgrade Areas

| Area | What changed |
|------|--------------|
| **Students** | Admin-only list/create; `GET /{id}` and `PUT`/`DELETE /{id}` limited to admin or the student themselves; `me`, `my-enrollments`, `enrollment-requests` added. Students can no longer change their own `isActive` or login email |
| **Instructors** | Previously fully anonymous. Now admin-only list/create; an instructor sees and edits only their own profile; `my-tracks` added |
| **Training tracks** | Create/delete admin-only; update limited to the assigned instructor; students browse `available` published tracks; `assign-instructor` and `progress` added |
| **Enrollments** | Previously fully anonymous. Now admin sees all; students see and create only their own; instructors read only their own tracks' students |
| **Payments** | Admin-only list/create/status; students read only their own payments; instructors have no revenue access; staff `notes` hidden from students |
| **Reports** | Admin-only (`[Authorize(Roles = "Admin")]` on the controller); an instructor-scoped progress report was added under tracks |

---

## 🔑 Ownership Rule

> A student token must never be able to pass another student's id and retrieve private data. Use the identity from claims, not user-submitted ids.

| Mechanism | Where it applies |
|-----------|------------------|
| **Identity from the token only** — `User.GetUserId()` reads the `NameIdentifier` claim | `me`, `my-enrollments`, `my-payments`, `my-tracks`, `enrollment-requests` take **no id from the client at all** |
| **Compare the route id with the token's id** → `403` on mismatch | `GET/PUT/DELETE /api/students/{id}`, `GET /api/instructors/{id}`, `GET /api/enrollments/student/{studentId}` |
| **Load the record, then compare its owner** → `403` on mismatch | `GET /api/enrollments/{id}`, `GET /api/payments/enrollment/{id}` (student must own the enrollment); track endpoints (instructor must be assigned to the track) |
| **Never trust a client-sent actor** | `PUT /api/tracks/{id}` overwrites `InstructorId` with the track's real owner after the check; `DELETE /api/tracks/{id}` no longer takes `?instructorId=` |

Test: with a **student** token, `GET /api/students/{otherStudentId}` → `403`. With an **instructor** token, `GET /api/tracks/{otherInstructorsTrackId}/students` → `403`.

