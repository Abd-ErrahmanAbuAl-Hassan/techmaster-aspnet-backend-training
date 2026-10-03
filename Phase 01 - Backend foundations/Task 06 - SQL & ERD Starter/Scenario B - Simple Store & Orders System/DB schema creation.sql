create database StoreOrdersDB;

USE StoreOrdersDB;


create table Customers (
    CustomerId   int primary key identity(1,1),
    FullName     nvarchar(100) not null,
    Email        nvarchar(100) not null unique,
    PhoneNumber  nvarchar(20),
    CreatedAt    datetime      not null default GETDATE()
);


create table Categories (
    CategoryId   int primary key identity(1,1),
    Name         nvarchar(50)  not null unique,
    Description  nvarchar(200)
);


create table Suppliers (
    SupplierId   int primary key identity(1,1),
    Name         nvarchar(100) not null,
    PhoneNumber  nvarchar(20),
    Email        nvarchar(100) unique
);


create table Products (
    ProductId      int primary key identity(1,1),
    Name           nvarchar(100)  not null,
    Price          decimal(10,2)  not null check (Price >= 0),
    StockQuantity  int            not null default 0 check (StockQuantity >= 0),
    CategoryId     int            not null,
    SupplierId     int            not null,
    IsAvailable    BIT            not null default 1,

    constraint FK_Products_Categories foreign key (CategoryId) references Categories(CategoryId),
    constraint FK_Products_Suppliers foreign key (SupplierId) references Suppliers(SupplierId)
);


create table Orders (
    OrderId      int primary key identity(1,1),
    CustomerId   int            not null,
    OrderDate    datetime       not null default GETDATE(),
    Status       nvarchar(20)   not null default N'Pending'
                 check (Status in (N'Pending', N'Shipped', N'Delivered', N'Cancelled')),
    TotalAmount  decimal(10,2)  not null default 0 check (TotalAmount >= 0),

    constraint FK_Orders_Customers foreign key (CustomerId) references Customers(CustomerId)
);

create table OrderItems (
    OrderItemId  int primary key identity(1,1),
    OrderId      int            not null,
    ProductId    int            not null,
    Quantity     int            not null default 1 check (Quantity > 0),
    UnitPrice    decimal(10,2)  not null check (UnitPrice >= 0),

    constraint FK_OrderItems_Orders foreign key (OrderId)   references Orders(OrderId),
    constraint FK_OrderItems_Products foreign key (ProductId) references Products(ProductId),
);

