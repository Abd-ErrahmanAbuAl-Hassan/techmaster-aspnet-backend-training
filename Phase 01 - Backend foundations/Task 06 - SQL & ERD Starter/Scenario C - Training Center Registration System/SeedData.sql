USE TrainingCenterDB;

INSERT INTO Instructors (FullName, Email, Specialization) VALUES
(N'Ahmed Fathy',      N'ahmed.fathy@center.com',      N'Web Development'),
(N'Mona Kamal',       N'mona.kamal@center.com',       N'Data Science'),
(N'Khaled Samir',     N'khaled.samir@center.com',     N'Mobile Development'),
(N'Sara Adel',        N'sara.adel@center.com',        N'UI/UX Design'),
(N'Hassan Ibrahim',   N'hassan.ibrahim@center.com',   N'Cybersecurity'),
(N'Yasmin Nabil',     N'yasmin.nabil@center.com',     N'Cloud & DevOps'),
(N'Tarek Mostafa',    N'tarek.mostafa@center.com',    N'Game Development');

INSERT INTO Students (FullName, Email, PhoneNumber, CreatedAt) VALUES
(N'Ali Mahmoud',      N'ali.mahmoud@example.com',      N'01011111111', '2024-01-10'),
(N'Fatma Hassan',     N'fatma.hassan@example.com',     N'01022222222', '2024-02-05'),
(N'Omar Sherif',      N'omar.sherif@example.com',      N'01033333333', '2024-02-20'),
(N'Nour ElSayed',     N'nour.elsayed@example.com',     N'01044444444', '2024-03-12'),
(N'Mahmoud Adel',     N'mahmoud.adel@example.com',     N'01055555555', '2024-04-01'),
(N'Reem Khalid',      N'reem.khalid@example.com',      N'01066666666', '2024-04-18'),
(N'Karim Tarek',      N'karim.tarek@example.com',      N'01077777777', '2024-05-09'),
(N'Dina Mostafa',     N'dina.mostafa@example.com',     N'01088888888', '2024-06-22'),
(N'Hana Youssef',     N'hana.youssef@example.com',     N'01099999999', '2024-07-14'),
(N'Youssef Emad',     N'youssef.emad@example.com',     N'01000000000', '2024-08-03');

INSERT INTO Tracks (Title, Description, DurationWeeks, StartDate, InstructorId) VALUES
(N'Full-Stack Web Development',   N'HTML, CSS, JS, Node.js and React from scratch',      12, '2024-09-01', 1),
(N'Front-End with React',         N'Modern React, hooks and state management',            8, '2024-09-15', 1),
(N'Data Science Bootcamp',        N'Python, Pandas, ML fundamentals and projects',       16, '2024-09-10', 2),
(N'Machine Learning Advanced',    N'Deep learning, NLP and model deployment',            12, '2024-10-05', 2),
(N'Android with Kotlin',          N'Native Android development with Jetpack Compose',   10, '2024-09-20', 3),
(N'iOS with Swift',               N'SwiftUI, Combine and App Store publishing',          10, '2024-10-15', 3),
(N'UI/UX Design Fundamentals',    N'Design thinking, Figma and prototyping',              6, '2024-09-05', 4),
(N'Advanced UI/UX & Design System', N'Design systems, accessibility and motion',         6, '2024-10-20', 4),
(N'Cybersecurity Essentials',     N'Network security, ethical hacking basics',            8, '2024-09-25', 5),
(N'Cloud & DevOps Engineering',   N'AWS, Docker, Kubernetes and CI/CD pipelines',        14, '2024-10-01', 6),
(N'Game Development with Unity',  N'C#, Unity engine and 2D/3D game projects',           12, '2024-10-10', 7);

INSERT INTO Registrations (StudentId, TrackId, RegistrationDate, Status) VALUES
(1,  1,  '2024-08-15', N'Confirmed'),
(2,  1,  '2024-08-16', N'Confirmed'),
(3,  1,  '2024-08-20', N'Pending'),
(4,  2,  '2024-08-25', N'Confirmed'),
(5,  3,  '2024-08-28', N'Confirmed'),
(6,  3,  '2024-09-01', N'Confirmed'),
(7,  3,  '2024-09-02', N'Cancelled'),
(8,  4,  '2024-09-18', N'Pending'),
(9,  5,  '2024-09-05', N'Confirmed'),
(10, 5,  '2024-09-06', N'Confirmed'),
(1,  7,  '2024-08-22', N'Confirmed'),
(2,  7,  '2024-08-23', N'Confirmed'),
(3,  9,  '2024-09-10', N'Confirmed'),
(4,  9,  '2024-09-11', N'Pending'),
(5,  10, '2024-09-15', N'Confirmed'),
(6,  10, '2024-09-16', N'Confirmed'),
(7,  11, '2024-09-25', N'Confirmed'),
(8,  11, '2024-09-26', N'Pending'),
(9,  6,  '2024-10-01', N'Confirmed'),
(10, 8,  '2024-10-08', N'Confirmed'),
(1,  10, '2024-09-30', N'Completed'),
(2,  1,  '2024-08-18', N'Completed');

INSERT INTO Payments (RegistrationId, Amount, PaymentDate, PaymentStatus) VALUES
(1,  3500.00, '2024-08-15', N'Paid'),
(2,  3500.00, '2024-08-16', N'Paid'),
(3,  3500.00, '2024-08-20', N'Pending'),
(4,  2800.00, '2024-08-25', N'Paid'),
(5,  4500.00, '2024-08-28', N'Paid'),
(6,  4500.00, '2024-09-01', N'Paid'),
(7,  4500.00, '2024-09-02', N'Refunded'),
(8,  5000.00, '2024-09-18', N'Pending'),
(9,  3200.00, '2024-09-05', N'Paid'),
(10, 3200.00, '2024-09-06', N'Paid'),
(11, 1500.00, '2024-08-22', N'Paid'),
(12, 1500.00, '2024-08-23', N'Paid'),
(13, 2200.00, '2024-09-10', N'Paid'),
(14, 2200.00, '2024-09-11', N'Pending'),
(15, 4800.00, '2024-09-15', N'Paid'),
(16, 4800.00, '2024-09-16', N'Paid'),
(17, 4000.00, '2024-09-25', N'Paid'),
(18, 4000.00, '2024-09-26', N'Failed'),
(19, 3000.00, '2024-10-01', N'Paid'),
(20, 1500.00, '2024-10-08', N'Paid'),
(21, 4800.00, '2024-09-30', N'Paid'),
(22, 3500.00, '2024-08-18', N'Paid');
