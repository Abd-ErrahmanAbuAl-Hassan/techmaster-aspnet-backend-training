USE StoreOrdersDB;

INSERT INTO Categories (Name, Description) VALUES
(N'Electronics',   N'Devices, gadgets and accessories'),
(N'Clothing',      N'Men, women and kids apparel'),
(N'Home & Kitchen',N'Home appliances and kitchenware'),
(N'Books',         N'Printed and digital books'),
(N'Sports',        N'Sports equipment and outdoor gear'),
(N'Toys',          N'Toys and games for children');

INSERT INTO Suppliers (Name, PhoneNumber, Email) VALUES
(N'TechSource Ltd',      N'01001234567', N'sales@techsource.com'),
(N'FashionHub Co',       N'01112345678', N'orders@fashionhub.com'),
(N'HomeStyle Trading',   N'01223456789', N'info@homestyle.com'),
(N'BookWorld Distributors', N'01034567890', N'contact@bookworld.com'),
(N'SportPro Supplies',   N'01145678901', N'support@sportpro.com'),
(N'FunToys Inc',         N'01256789012', N'hello@funtoys.com');

INSERT INTO Customers (FullName, Email, PhoneNumber, CreatedAt) VALUES
(N'Ahmed Ali',       N'ahmed.ali@example.com',      N'01011111111', '2024-01-05'),
(N'Sara Mohamed',    N'sara.mohamed@example.com',   N'01022222222', '2024-02-11'),
(N'Omar Hassan',     N'omar.hassan@example.com',    N'01033333333', '2024-03-19'),
(N'Mona Ibrahim',    N'mona.ibrahim@example.com',   N'01044444444', '2024-04-02'),
(N'Khaled Youssef',  N'khaled.youssef@example.com', N'01055555555', '2024-05-27'),
(N'Layla Samir',     N'layla.samir@example.com',    N'01066666666', '2024-06-14'),
(N'Youssef Adel',    N'youssef.adel@example.com',   N'01077777777', '2024-07-30'),
(N'Nourhan Tarek',   N'nourhan.tarek@example.com',  N'01088888888', '2024-08-21');

INSERT INTO Products (Name, Price, StockQuantity, CategoryId, SupplierId, IsAvailable) VALUES
-- Electronics (CategoryId = 1, SupplierId = 1)
(N'Wireless Mouse',            250.00,  120, 1, 1, 1),
(N'Mechanical Keyboard',       950.00,   45, 1, 1, 1),
(N'USB-C Charger 65W',         420.00,   80, 1, 1, 1),
(N'Bluetooth Headphones',     1300.00,   30, 1, 1, 1),
(N'4K Monitor 27"',           8500.00,    8, 1, 1, 1),

-- Clothing (CategoryId = 2, SupplierId = 2)
(N'Cotton T-Shirt',            180.00,  200, 2, 2, 1),
(N'Slim Fit Jeans',            550.00,  100, 2, 2, 1),
(N'Winter Jacket',            1450.00,   25, 2, 2, 1),
(N'Running Shoes',            1200.00,   60, 2, 2, 1),

-- Home & Kitchen (CategoryId = 3, SupplierId = 3)
(N'Non-Stick Frying Pan',      320.00,   75, 3, 3, 1),
(N'Blender 1.5L',              890.00,   40, 3, 3, 1),
(N'Ceramic Dinner Set (12pc)', 1650.00,   18, 3, 3, 1),
(N'Coffee Maker',              740.00,   35, 3, 3, 1),

-- Books (CategoryId = 4, SupplierId = 4)
(N'Clean Code',                480.00,   50, 4, 4, 1),
(N'The Pragmatic Programmer',  520.00,   40, 4, 4, 1),
(N'Atomic Habits',             310.00,   90, 4, 4, 1),
(N'Rich Dad Poor Dad',         270.00,   70, 4, 4, 1),

-- Sports (CategoryId = 5, SupplierId = 5)
(N'Yoga Mat',                  260.00,   85, 5, 5, 1),
(N'Dumbbell Set 20kg',        1850.00,   15, 5, 5, 1),
(N'Football (Size 5)',         340.00,  110, 5, 5, 1),
(N'Water Bottle 1L',            120.00,  300, 5, 5, 1),

-- Toys (CategoryId = 6, SupplierId = 6)
(N'Building Blocks 500pcs',    650.00,   40, 6, 6, 1),
(N'Remote Control Car',        880.00,   22, 6, 6, 1),
(N'Puzzle 1000pcs',            230.00,   65, 6, 6, 1),
(N'Board Game Classic',        450.00,    0, 6, 6, 0);  

INSERT INTO Orders (CustomerId, OrderDate, Status, TotalAmount) VALUES
(1, '2024-08-01', N'Delivered', 1200.00),
(2, '2024-08-05', N'Delivered',  550.00),
(3, '2024-08-12', N'Shipped',   8500.00),
(4, '2024-08-18', N'Pending',    740.00),
(5, '2024-09-01', N'Delivered', 1950.00),
(1, '2024-09-04', N'Cancelled',    0.00),
(6, '2024-09-09', N'Delivered',  890.00),
(7, '2024-09-15', N'Shipped',   1200.00),
(2, '2024-09-20', N'Pending',    310.00),
(8, '2024-09-25', N'Delivered', 1040.00);

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES
-- Order 1 (Customer 1) : Mouse + Keyboard
(1, 1, 2, 250.00),
(1, 2, 1, 950.00)  

-- Order 2 (Customer 2) : Jeans x1
, (2, 7, 1, 550.00)

-- Order 3 (Customer 3) : Monitor x1
, (3, 5, 1, 8500.00)

-- Order 4 (Customer 4) : Coffee Maker x1
, (4, 13, 1, 740.00)

-- Order 5 (Customer 5) : Running Shoes + Yoga Mat + Water Bottle
, (5, 9, 1, 1200.00)
, (5, 18, 2, 260.00)
, (5, 21, 1, 120.00)

-- Order 6 cancelled -> no items

-- Order 7 (Customer 6) : Blender x1
, (7, 11, 1, 890.00)

-- Order 8 (Customer 7) : Running Shoes x1
, (8, 9, 1, 1200.00)

-- Order 9 (Customer 2) : Atomic Habits x1
, (9, 16, 1, 310.00)

-- Order 10 (Customer 8) : Headphones + Water Bottle
, (10, 4, 1, 1300.00)
, (10, 21, 2, 120.00);
