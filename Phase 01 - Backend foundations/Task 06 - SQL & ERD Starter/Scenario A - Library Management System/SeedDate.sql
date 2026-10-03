use BookStore;

-- =========================
-- Seed Authors
-- =========================
INSERT INTO Authors (FullName, BirthDate, Country) VALUES
(N'George Orwell',        '1903-06-25', N'United Kingdom'),
(N'J.K. Rowling',         '1965-07-31', N'United Kingdom'),
(N'Stephen King',         '1947-09-21', N'United States'),
(N'Agatha Christie',      '1890-09-15', N'United Kingdom'),
(N'Naguib Mahfouz',       '1911-12-11', N'Egypt'),
(N'Paulo Coelho',         '1947-08-24', N'Brazil'),
(N'Toni Morrison',        '1931-02-18', N'United States'),
(N'Haruki Murakami',      '1949-01-12', N'Japan');

-- =========================
-- Seed Categories
-- =========================
INSERT INTO Categories (Name, Description) VALUES
(N'Fiction',        N'Fictional literature and novels'),
(N'Fantasy',        N'Fantasy and magical worlds'),
(N'Horror',         N'Horror and thriller books'),
(N'Mystery',        N'Detective and mystery novels'),
(N'Drama',          N'Dramatic literary works'),
(N'Philosophical',  N'Philosophical and reflective works'),
(N'Historical',     N'Historical fiction and non-fiction');

-- =========================
-- Seed Members
-- =========================
INSERT INTO Members (FullName, Email, PhoneNumber, JoinDate, IsActive) VALUES
(N'Ahmed Ali',        N'ahmed.ali@example.com',      N'01001234567', '2023-01-15', 1),
(N'Sara Mohamed',     N'sara.mohamed@example.com',   N'01112345678', '2023-03-22', 1),
(N'Omar Hassan',      N'omar.hassan@example.com',    N'01223456789', '2023-05-10', 1),
(N'Mona Ibrahim',     N'mona.ibrahim@example.com',   N'01034567890', '2023-07-04', 1),
(N'Khaled Youssef',   N'khaled.youssef@example.com', N'01145678901', '2024-01-18', 1),
(N'Layla Samir',      N'layla.samir@example.com',    N'01256789012', '2024-02-27', 0),
(N'Youssef Adel',     N'youssef.adel@example.com',   N'01067890123', '2024-06-12', 1);

-- =========================
-- Seed Books
-- =========================
INSERT INTO Books (Title, PublishYear, ISBN, AvailableCopies, AuthorId, CategoryId) VALUES
(N'1984',                     '1949-06-08', N'978-0451524935', 1, 1, 1),
(N'Animal Farm',              '1945-08-17', N'978-0451526342', 1, 1, 1),
(N'Harry Potter and the Philosopher''s Stone', '1997-06-26', N'978-0747532699', 1, 2, 2),
(N'Harry Potter and the Chamber of Secrets',  '1998-07-02', N'978-0747538493', 1, 2, 2),
(N'The Shining',              '1977-01-28', N'978-0307743657', 1, 3, 3),
(N'It',                       '1986-09-15', N'978-1501142970', 0, 3, 3),
(N'Murder on the Orient Express', '1934-01-01', N'978-0062693662', 1, 4, 4),
(N'And Then There Were None', '1939-11-06', N'978-0062073488', 1, 4, 4),
(N'Palace Walk',              '1956-01-01', N'978-0385264662', 1, 5, 7),
(N'The Alchemist',            '1988-01-01', N'978-0062315007', 1, 6, 6),
(N'Beloved',                  '1987-09-16', N'978-1400033416', 1, 7, 5),
(N'Norwegian Wood',           '1987-09-04', N'978-0375704024', 1, 8, 1),
(N'Kafka on the Shore',       '2002-09-12', N'978-1400079278', 1, 8, 2);

-- =========================
-- Seed BorrowRecords
-- =========================
INSERT INTO BorrowRecords (BorrowDate, DueDate, ReturnDate, Status, MemberId, BookId) VALUES
('2024-08-01', '2024-08-15', '2024-08-12', 1, 1, 1),   -- returned on time
('2024-08-05', '2024-08-19', NULL,         0, 2, 3),   -- currently borrowed
('2024-08-10', '2024-08-24', '2024-08-30', 1, 3, 5),   -- returned late
('2024-08-12', '2024-08-26', '2024-08-20', 1, 4, 7),   -- returned early
('2024-09-01', '2024-09-15', NULL,         0, 5, 9),   -- currently borrowed
('2024-09-03', '2024-09-17', NULL,         0, 1, 10),  -- currently borrowed
('2024-09-05', '2024-09-19', '2024-09-18', 1, 2, 12),  -- returned
('2024-09-10', '2024-09-24', NULL,         0, 3, 2),   -- currently borrowed
('2024-09-12', '2024-09-26', '2024-09-25', 1, 7, 13),  -- returned
('2024-09-15', '2024-09-29', NULL,         0, 4, 4);   -- currently borrowed