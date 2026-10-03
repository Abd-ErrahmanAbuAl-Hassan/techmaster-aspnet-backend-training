
-- 1. Select all books
SELECT * FROM Books;

-- 2. Select all active members
SELECT * FROM Members
WHERE IsActive = 1;

-- 3. Select books by category
-- By Name
SELECT b.* FROM Books b INNER JOIN Categories c 
ON b.CategoryId = c.CategoryId
WHERE c.Name = N'Fantasy';
-- Or by ID
SELECT * FROM Books
WHERE CategoryId = 2;

-- 4. Count books per category
SELECT c.CategoryId, c.Name AS CategoryName, COUNT(b.BookId) AS BookCount
FROM Categories c LEFT JOIN Books b
ON b.CategoryId = c.CategoryId
group by c.CategoryId , c.Name

-- 5. Select borrow records with member name and book title (JOIN)
SELECT
    br.Id,
    m.FullName      AS MemberName,
    b.Title         AS BookTitle,
    br.BorrowDate,
    br.DueDate,
    br.ReturnDate,
    br.Status
FROM BorrowRecords br
INNER JOIN Members m ON br.MemberId = m.MemberId
INNER JOIN Books   b ON br.BookId   = b.BookId

-- 6. Select overdue books
SELECT
    br.Id,
    m.FullName   AS MemberName,
    m.Email,
    b.Title      AS BookTitle,
    br.BorrowDate,
    br.DueDate,
    DATEDIFF(DAY, br.DueDate, GETDATE()) AS DaysOverdue
FROM BorrowRecords br
INNER JOIN Members m ON br.MemberId = m.MemberId
INNER JOIN Books   b ON br.BookId   = b.BookId
WHERE br.ReturnDate IS NULL
  AND br.DueDate < GETDATE()

-- 7. Select borrowing history for one member
SELECT
    br.Id,
    m.FullName  AS MemberName,
    b.Title     AS BookTitle,
    br.BorrowDate,
    br.DueDate,
    br.ReturnDate,
    br.Status
FROM BorrowRecords br
INNER JOIN Members m ON br.MemberId = m.MemberId
INNER JOIN Books   b ON br.BookId   = b.BookId
WHERE br.MemberId = 1

-- 8. Select available books
SELECT b.BookId, b.Title, b.PublishYear, b.ISBN,
       a.FullName AS AuthorName,
       c.Name     AS CategoryName
FROM Books b
INNER JOIN Authors    a ON b.AuthorId   = a.AuthorId
INNER JOIN Categories c ON b.CategoryId = c.CategoryId
WHERE b.AvailableCopies = 1

-- 9. Count how many books each author has
SELECT a.AuthorId, a.FullName AS AuthorName, COUNT(b.BookId) AS BookCount
FROM Authors a
LEFT JOIN Books b ON b.AuthorId = a.AuthorId
GROUP BY a.AuthorId, a.FullName

-- 10. Select top 5 most borrowed books
SELECT TOP 5
    b.BookId,
    b.Title,
    COUNT(br.Id) AS TimesBorrowed
FROM Books b
LEFT JOIN BorrowRecords br ON br.BookId = b.BookId
GROUP BY b.BookId, b.Title

