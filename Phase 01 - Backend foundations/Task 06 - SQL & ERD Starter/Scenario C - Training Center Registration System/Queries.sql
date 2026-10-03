USE TrainingCenterDB;


-- 1. Select all students
SELECT * FROM Students

-- 2. Select all tracks
SELECT TrackId, Title, Description, DurationWeeks, StartDate, InstructorId
FROM Tracks

-- 3. Select students registered in a specific track
SELECT
    s.StudentId,
    s.FullName,
    s.Email,
    s.PhoneNumber,
    r.RegistrationDate,
    r.Status AS RegistrationStatus
FROM Registrations r
INNER JOIN Students s ON s.StudentId = r.StudentId
WHERE r.TrackId = 1

-- 4. Count students per track
SELECT
    t.TrackId,
    t.Title AS TrackTitle,
    COUNT(r.RegistrationId) AS StudentCount
FROM Tracks t
LEFT JOIN Registrations r ON r.TrackId = t.TrackId
GROUP BY t.TrackId, t.Title

-- 5. Select unpaid registrations
SELECT
    r.RegistrationId,
    s.FullName       AS StudentName,
    t.Title          AS TrackTitle,
    r.RegistrationDate,
    r.Status         AS RegistrationStatus,
    p.PaymentId,
    ISNULL(p.Amount, 0)        AS Amount,
    ISNULL(p.PaymentStatus, N'No Payment') AS PaymentStatus
FROM Registrations r
INNER JOIN Students s ON s.StudentId = r.StudentId
INNER JOIN Tracks   t ON t.TrackId   = r.TrackId
LEFT  JOIN Payments p ON p.RegistrationId = r.RegistrationId
WHERE p.PaymentId IS NULL
   OR p.PaymentStatus <> N'Paid'

-- 6. Select tracks by instructor
SELECT
    t.TrackId,
    t.Title,
    t.DurationWeeks,
    t.StartDate,
    i.FullName AS InstructorName,
    i.Specialization
FROM Tracks t
INNER JOIN Instructors i ON i.InstructorId = t.InstructorId
WHERE t.InstructorId = 1

-- 7. Select registrations with payment status using JOIN
SELECT
    r.RegistrationId,
    s.FullName  AS StudentName,
    t.Title     AS TrackTitle,
    r.RegistrationDate,
    r.Status    AS RegistrationStatus,
    p.PaymentId,
    p.Amount,
    p.PaymentDate,
    ISNULL(p.PaymentStatus, N'No Payment') AS PaymentStatus
FROM Registrations r
INNER JOIN Students s ON s.StudentId = r.StudentId
INNER JOIN Tracks   t ON t.TrackId   = r.TrackId
LEFT  JOIN Payments p ON p.RegistrationId = r.RegistrationId

-- 8. Select tracks starting after a specific date
SELECT
    t.TrackId,
    t.Title,
    t.DurationWeeks,
    t.StartDate,
    i.FullName AS InstructorName
FROM Tracks t
INNER JOIN Instructors i ON i.InstructorId = t.InstructorId
WHERE t.StartDate > '2024-10-01'

-- 9. Count tracks per instructor
SELECT
    i.InstructorId,
    i.FullName AS InstructorName,
    i.Specialization,
    COUNT(t.TrackId) AS TrackCount
FROM Instructors i
LEFT JOIN Tracks t ON t.InstructorId = i.InstructorId
GROUP BY i.InstructorId, i.FullName, i.Specialization

-- 10. Select student registration history
SELECT
    r.RegistrationId,
    s.FullName       AS StudentName,
    t.Title          AS TrackTitle,
    i.FullName       AS InstructorName,
    r.RegistrationDate,
    r.Status         AS RegistrationStatus,
    ISNULL(p.Amount, 0)        AS Amount,
    ISNULL(p.PaymentStatus, N'No Payment') AS PaymentStatus
FROM Registrations r
INNER JOIN Students    s ON s.StudentId    = r.StudentId
INNER JOIN Tracks      t ON t.TrackId      = r.TrackId
INNER JOIN Instructors i ON i.InstructorId = t.InstructorId
LEFT  JOIN Payments    p ON p.RegistrationId = r.RegistrationId
WHERE r.StudentId = 1
