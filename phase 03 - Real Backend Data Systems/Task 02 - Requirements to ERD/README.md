# TechMaster Academy — Backend Database Design (Task 02: Requirements → ERD)

## Overview

This document translates the TechMaster Academy business requirements into a relational database design. It covers the five core entities (Student, Instructor, TrainingTrack, Enrollment, Payment) plus the bonus entities (Sessions, Assignments, Attendance, Submission), their fields, keys, relationships, business rules, and how the design answers the required business questions.


![ERD Diagram](https://drive.google.com/file/d/1fWkza9Ro6iMcTkdgmEPnh0f6ObpQreY0/view?usp=drive_link)
![Mapping](https://drive.google.com/file/d/1wEzs4QAiSXXttDQn4D98K8k8P83oCehK/view?usp=drive_link)

---

## 1. Entities

| Entity | Purpose |
|---|---|
| **Student** | A person enrolled in one or more training tracks. |
| **Instructor** | A person who teaches one or more training tracks. |
| **TrainingTrack** | A course/program offered by the academy (e.g. "ASP.NET Backend – Level 2"). |
| **Enrollment** | The link between a Student and a TrainingTrack, plus progress/status data. |
| **Payment** | A single payment transaction made against an Enrollment. |

---

## 2. Fields, Primary Keys & Foreign Keys

### Student
| Field | Type | Notes |
|---|---|---|
| StudentId | int/guid | **Primary Key** |
| FullName | string | required |
| Email | string | required, unique |
| PhoneNumber | string | optional/required — decision left to implementation |
| CreatedAt | datetime (UTC) | system-generated |
| UpdatedAt | datetime (UTC) | nullable |
| IsActive | bool | |
| IsDeleted | bool | supports soft delete |
| DeletedAt | datetime (UTC) | nullable |

### Instructor
| Field | Type | Notes |
|---|---|---|
| InstructorId | int/guid | **Primary Key** |
| FullName | string | required |
| Email | string | required, unique |
| Specialization | string | |
| Bio | string | optional |
| IsActive | bool | |
| CreatedAt | datetime (UTC) | system-generated |

### TrainingTrack
| Field | Type | Notes |
|---|---|---|
| TrainingTrackId | int/guid | **Primary Key** |
| Title | string | required |
| Code | string | unique |
| Description | string | |
| Level | string | e.g. Beginner/Intermediate/Advanced |
| Capacity | int | max number of students |
| StartDate | date | |
| EndDate | date | |
| Status | string | e.g. Planned/Active/Completed |
| **InstructorId** | int/guid | **Foreign Key → Instructor.InstructorId** |
| CreatedAt | datetime (UTC) | system-generated |
| IsDeleted | bool | supports soft delete |

### Enrollment *(associative entity)*
| Field | Type | Notes |
|---|---|---|
| EnrollmentId | int/guid | **Primary Key** |
| **StudentId** | int/guid | **Foreign Key → Student.StudentId** |
| **TrainingTrackId** | int/guid | **Foreign Key → TrainingTrack.TrainingTrackId** |
| EnrollmentDate | date | |
| Status | string | e.g. Active/Completed/Withdrawn |
| ProgressPercentage | decimal/int | 0–100 |
| FinalResult | string | optional, set once the track finishes |
| CreatedAt | datetime (UTC) | system-generated |
| UpdatedAt | datetime (UTC) | nullable |

### Payment
| Field | Type | Notes |
|---|---|---|
| PaymentId | int/guid | **Primary Key** |
| **EnrollmentId** | int/guid | **Foreign Key → Enrollment.EnrollmentId** |
| Amount | decimal | never use float/double for money |
| PaymentMethod | string | e.g. Cash/Card/Transfer |
| PaymentDate | date | |
| PaymentStatus | string | e.g. Pending/Completed/Failed |
| ReferenceNumber | string | external transaction reference |
| Notes | string | optional |

### Sessions 
| Field | Type | Notes |
|---|---|---|
| SessionId | int/guid | **Primary Key** |
| **TrainingTrackId** | int/guid | **Foreign Key → TrainingTrack.TrainingTrackId** |
| SessionDate | date | |
| StartTime | time | |
| EndTime | time | |
| Topic | string | |

### Assignments 
| Field | Type | Notes |
|---|---|---|
| AssignmentId | int/guid | **Primary Key** |
| **TrainingTrackId** | int/guid | **Foreign Key → TrainingTrack.TrainingTrackId** |
| Title | string | |
| DueDate | date | |
| MaxScore | int | |

### Attendance *(associative entity)*
| Field | Type | Notes |
|---|---|---|
| AttendanceId | int/guid | **Primary Key** |
| **EnrollmentId** | int/guid | **Foreign Key → Enrollment.EnrollmentId** |
| **SessionId** | int/guid | **Foreign Key → Sessions.SessionId** |
| Status | string | Present/Absent/Late |
| CheckInTime | datetime | optional |

### Submission *(associative entity)*
| Field | Type | Notes |
|---|---|---|
| SubmissionId | int/guid | **Primary Key** |
| **EnrollmentId** | int/guid | **Foreign Key → Enrollment.EnrollmentId** |
| **AssignmentId** | int/guid | **Foreign Key → Assignments.AssignmentId** |
| SubmittedAt | datetime | |
| Grade | decimal | optional, set after review |
| Status | string | Submitted/Late/Missing |

---

## 3. Relationships

| Relationship | Cardinality | Explanation |
|---|---|---|
| Instructor → TrainingTrack | 1 → M | One instructor can teach many tracks; each track has exactly one main instructor. |
| TrainingTrack → Enrollment | 1 → M | One track can have many enrollments; each enrollment belongs to exactly one track. |
| Student → Enrollment | 1 → M | One student can have many enrollments; each enrollment belongs to exactly one student. |
| Enrollment → Payment | 1 → M | One enrollment can have many payments (installments); each payment belongs to exactly one enrollment. |
| Student ↔ TrainingTrack | M ↔ M (via Enrollment) | A student can join many tracks and a track can have many students. This many-to-many relationship is resolved through the `Enrollment` table, which also stores enrollment-specific data (date, status, progress, result) that a plain junction table could not hold. |
| TrainingTrack → Sessions | 1 → M | One track can have many sessions; each session belongs to exactly one track. |
| TrainingTrack → Assignments | 1 → M | One track can have many assignments; each assignment belongs to exactly one track. |
| Enrollment ↔ Sessions | M ↔ M (via Attendance) | A student attends many sessions across their enrollment, and a session is attended by many enrolled students. Resolved through `Attendance`, which links to `Enrollment` (not Student directly) so a student can only be marked present for a session in a track they're actually enrolled in, and stores Status/CheckInTime. |
| Enrollment ↔ Assignments | M ↔ M (via Submission) | A student submits many assignments across their enrollment, and an assignment receives many submissions. Resolved through `Submission`, which links to `Enrollment` (not Student directly) for the same enrollment-guard reason, and stores SubmittedAt/Grade/Status. |

---

## 4. Business Rules

1. A student can enroll in many training tracks; a training track can have many students (many-to-many, resolved via Enrollment).
2. Each training track has exactly one main instructor; one instructor can teach many tracks.
3. A student may make multiple payments toward a single enrollment (e.g. installments).
4. Enrollment tracks its own date, status, progress percentage, and final result — independent of the student's or track's own fields.
5. Soft-delete is used for Student and TrainingTrack (`IsDeleted` / `DeletedAt`) rather than physically removing historical records, since enrollments and payments must remain traceable.
6. All system-generated timestamps use `DateTime.UtcNow` to avoid time-zone inconsistencies.
7. Money fields (`Payment.Amount`) are stored as `decimal`, never `float`/`double`, to avoid rounding errors.
8. All primary/foreign keys are strongly typed (int or GUID) — never stored as free-text strings.
9. Attendance and Submission are modeled as their own entities (not bare many-to-many lines) so each attendance/submission record can carry its own status, timestamp, and grade — and so both are constrained to the student's actual enrollment rather than floating loose against Student directly.

---

## 5. Business Questions the Database Must Answer

| # | Question | How the schema answers it | Query |
|---|---|---|
| 1 | Which students are enrolled in a specific track? | `Enrollment` filtered by `TrainingTrackId`, joined to `Student`. | SELECT
    s.StudentId,
    s.FullName,
    s.Email,
    e.EnrollmentDate,
    e.Status AS EnrollmentStatus
FROM Enrollment e
JOIN Student s ON s.StudentId = e.StudentId
WHERE e.TrainingTrackId = @TrainingTrackId
ORDER BY s.FullName;|
| 2 | Which tracks have available seats? | `TrainingTrack.Capacity` minus `COUNT(Enrollment)` grouped by `TrainingTrackId`. | SELECT
    t.TrainingTrackId,
    t.Title,
    t.Capacity,
    COUNT(e.EnrollmentId) AS EnrolledCount,
    t.Capacity - COUNT(e.EnrollmentId) AS AvailableSeats
FROM TrainingTrack t
LEFT JOIN Enrollment e
    ON e.TrainingTrackId = t.TrainingTrackId
    AND e.Status <> 'Cancelled'          -- don't count cancelled enrollments against capacity
WHERE t.IsDeleted = 0
GROUP BY t.TrainingTrackId, t.Title, t.Capacity
HAVING t.Capacity - COUNT(e.EnrollmentId) > 0
ORDER BY AvailableSeats DESC; |
| 3 | Which enrollments are unpaid? | `Enrollment` left-joined to `Payment`; enrollments with no completed payment covering the required amount, or `Payment.PaymentStatus` ≠ Completed. | SELECT
    e.EnrollmentId,
    s.FullName AS StudentName,
    t.Title AS TrackTitle
FROM Enrollment e
JOIN Student s ON s.StudentId = e.StudentId
JOIN TrainingTrack t ON t.TrainingTrackId = e.TrainingTrackId
WHERE NOT EXISTS (
    SELECT 1 FROM Payment p
    WHERE p.EnrollmentId = e.EnrollmentId
      AND p.PaymentStatus = 'Completed'
); |
| 4 | How much revenue did each track generate? | `SUM(Payment.Amount)` joined through `Enrollment.TrainingTrackId`, grouped by track. | SELECT
    t.TrainingTrackId,
    t.Title,
    COALESCE(SUM(p.Amount), 0) AS TotalRevenue
FROM TrainingTrack t
LEFT JOIN Enrollment e ON e.TrainingTrackId = t.TrainingTrackId
LEFT JOIN Payment p
    ON p.EnrollmentId = e.EnrollmentId
    AND p.PaymentStatus = 'Completed'
GROUP BY t.TrainingTrackId, t.Title
ORDER BY TotalRevenue DESC; |
| 5 | Which instructor has the highest workload? | `COUNT(TrainingTrack)` (or count of active enrollments across their tracks) grouped by `InstructorId`. | SELECT TOP 1
    i.InstructorId,
    i.FullName,
    COUNT(e.EnrollmentId) AS ActiveStudentCount
FROM Instructor i
JOIN TrainingTrack t ON t.InstructorId = i.InstructorId
JOIN Enrollment e ON e.TrainingTrackId = t.TrainingTrackId AND e.Status = 'Active'
GROUP BY i.InstructorId, i.FullName
ORDER BY ActiveStudentCount DESC; |
| 6 | Which students have active enrollments? | `Enrollment` filtered by `Status = 'Active'`, joined to `Student`. | SELECT DISTINCT
    s.StudentId,
    s.FullName,
    s.Email
FROM Student s
JOIN Enrollment e ON e.StudentId = s.StudentId
WHERE e.Status = 'Active'
ORDER BY s.FullName; |
| 7 | Which tracks start this month? | `TrainingTrack` filtered by `StartDate` within the current month. | SELECT
    t.TrainingTrackId,
    t.Title,
    t.StartDate,
    t.Status
FROM TrainingTrack t
WHERE YEAR(t.StartDate) = YEAR(GETUTCDATE())
  AND MONTH(t.StartDate) = MONTH(GETUTCDATE())
  AND t.IsDeleted = 0
ORDER BY t.StartDate; |
| 8 | What is the payment history for an enrollment? | `Payment` filtered by `EnrollmentId`, ordered by `PaymentDate`. | SELECT
    p.PaymentId,
    p.Amount,
    p.PaymentMethod,
    p.PaymentDate,
    p.PaymentStatus,
    p.ReferenceNumber
FROM Payment p
WHERE p.EnrollmentId = @EnrollmentId
ORDER BY p.PaymentDate; |
| 9 | Which tracks are full? | `TrainingTrack` where `COUNT(Enrollment)` = `Capacity`. | SELECT
    t.TrainingTrackId,
    t.Title,
    t.Capacity,
    COUNT(e.EnrollmentId) AS EnrolledCount
FROM TrainingTrack t
JOIN Enrollment e
    ON e.TrainingTrackId = t.TrainingTrackId
    AND e.Status <> 'Cancelled'
WHERE t.IsDeleted = 0
GROUP BY t.TrainingTrackId, t.Title, t.Capacity
HAVING COUNT(e.EnrollmentId) >= t.Capacity
ORDER BY t.Title; |
| 10 | How many enrollments exist by status? | `COUNT(Enrollment)` grouped by `Status`. | SELECT
    e.Status,
    COUNT(*) AS EnrollmentCount
FROM Enrollment e
GROUP BY e.Status
ORDER BY EnrollmentCount DESC; |

---

## 6. Design Decisions

- **Enrollment as a junction table with attributes.** Student and TrainingTrack have a natural many-to-many relationship, but the business requires extra data per enrollment (date, status, progress, result). A plain many-to-many join table can't hold that data, so `Enrollment` is modeled as a full entity with its own primary key rather than a composite-key join table.
- **Payment linked to Enrollment, not Student.** Payments are described as being tied to enrollments, and a student can pay in installments per enrollment — so `Payment.EnrollmentId` is the correct foreign key rather than `Payment.StudentId`. Student-level payment history is still derivable by joining through Enrollment.
- **One main instructor per track.** The requirement states each track has *one* main instructor, so `InstructorId` lives on `TrainingTrack` as a single foreign key rather than a many-to-many instructor/track table. (If co-instructors become a requirement later, this would need a separate join table.)
- **Soft delete over hard delete.** Students and tracks can be deactivated (`IsDeleted`, `DeletedAt`) instead of removed, since enrollment and payment history must remain intact for reporting (revenue, workload, etc.) even after a student or track is no longer active.
- **Decimal for money, UTC for dates.** Prevents floating-point rounding errors on `Payment.Amount` and time-zone bugs on all system-generated timestamps.
- **Surrogate keys, not stringly-typed IDs.** All primary/foreign keys are integers or GUIDs, keeping joins fast and type-safe rather than relying on business codes (like `Code` or `Email`) as keys.
- **Person as a disjoint superclass of Student/Instructor.** A person can be a student or an instructor, but never both at once ("d" = disjoint specialization). This avoids duplicating shared fields like FullName/Email across Student and Instructor if the school ever needs a unified login or contact table.
- **Attendance and Submission replace the earlier bare M:M lines.** An early draft connected Student directly to Sessions ("attend") and to Assignments ("take") as plain many-to-many relationships. That can't store per-record data (attendance status, submission grade) and doesn't stop a student from "attending" a session for a track they never enrolled in. Routing both through `Enrollment` — the same associative-entity pattern already used to resolve Student↔TrainingTrack — fixes both problems and keeps the design consistent.

---

