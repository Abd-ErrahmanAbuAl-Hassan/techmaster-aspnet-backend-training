USE StoreOrdersDB;

-- 1. Select all products
SELECT * FROM Products

-- 2. Select available products
SELECT p.ProductId, p.Name, p.Price, p.StockQuantity,
       c.Name AS CategoryName,
       s.Name AS SupplierName
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.CategoryId
INNER JOIN Suppliers  s ON p.SupplierId = s.SupplierId
WHERE p.IsAvailable = 1
  AND p.StockQuantity > 0

-- 3. Select products by category
-- By Name
SELECT p.ProductId, p.Name, p.Price, p.StockQuantity, p.IsAvailable
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.CategoryId
WHERE c.Name = N'Electronics'
-- Or ID
SELECT * FROM Products
WHERE CategoryId = 1;

-- 4. Select products with low stock
SELECT p.ProductId, p.Name, p.StockQuantity,
       c.Name AS CategoryName,
       s.Name AS SupplierName
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.CategoryId
INNER JOIN Suppliers  s ON p.SupplierId = s.SupplierId
WHERE p.StockQuantity <= 20

-- 5. Select orders for one customer
SELECT o.OrderId, o.OrderDate, o.Status, o.TotalAmount
FROM Orders o
WHERE o.CustomerId = 1

-- 6. Select order details using JOIN
SELECT
    o.OrderId,
    o.OrderDate,
    o.Status,
    c.FullName   AS CustomerName,
    p.Name       AS ProductName,
    oi.Quantity,
    oi.UnitPrice,
    (oi.Quantity * oi.UnitPrice) AS LineTotal
FROM Orders o
INNER JOIN Customers  c  ON o.CustomerId  = c.CustomerId
INNER JOIN OrderItems oi ON o.OrderId     = oi.OrderId
INNER JOIN Products   p  ON oi.ProductId  = p.ProductId

-- 7. Calculate total sales
SELECT SUM(oi.Quantity * oi.UnitPrice) AS TotalSales
FROM OrderItems oi
INNER JOIN Orders o ON oi.OrderId = o.OrderId
WHERE o.Status <> N'Cancelled';

-- 8. Count products per category
SELECT c.CategoryId, c.Name AS CategoryName,
       COUNT(p.ProductId) AS ProductCount
FROM Categories c
LEFT JOIN Products p ON p.CategoryId = c.CategoryId
GROUP BY c.CategoryId, c.Name

-- 9. Select best-selling products
SELECT TOP 10
    p.ProductId,
    p.Name AS ProductName,
    SUM(oi.Quantity) AS TotalQuantitySold,
    SUM(oi.Quantity * oi.UnitPrice) AS TotalRevenue
FROM Products p
INNER JOIN OrderItems oi ON p.ProductId = oi.ProductId
INNER JOIN Orders     o  ON oi.OrderId  = o.OrderId
WHERE o.Status <> N'Cancelled'
GROUP BY p.ProductId, p.Name

-- 10. Select suppliers with their products
SELECT
    s.SupplierId,
    s.Name          AS SupplierName,
    s.PhoneNumber,
    s.Email,
    p.ProductId,
    p.Name          AS ProductName,
    p.Price,
    p.StockQuantity,
    p.IsAvailable
FROM Suppliers s
LEFT JOIN Products p ON p.SupplierId = s.SupplierId
