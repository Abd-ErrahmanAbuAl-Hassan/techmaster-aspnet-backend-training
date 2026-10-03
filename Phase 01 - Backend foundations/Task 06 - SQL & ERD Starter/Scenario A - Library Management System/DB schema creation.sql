create database BookStore;

use BookStore;

create table Authors (

    AuthorId    int primary key identity(1,1),
    FullName    nvarchar(100) not null,
    BirthDate   date          null,
    Country     nvarchar(60)  not null default N'Unknown',

);

create table Categories (

    CategoryId   int primary key identity(1,1),
    Name         nvarchar(60)  not null unique,
    Description  nvarchar(200) null
);

create table Books (

    BookId           int primary key identity(1,1),
    Title            nvarchar(150) not null,
    PublishYear      date          null,
    ISBN             nvarchar(20)  not null unique,
    AvailableCopies  bit           not null,

    AuthorId         int           not null,
    CategoryId       int           not null,

    constraint fk_Books_Authors foreign key (AuthorId)   references Authors(AuthorId),
    constraint fk_Books_Categories foreign key (CategoryId) references Categories(CategoryId),
);

create table Members (

    MemberId     int primary key identity(1,1),
    FullName     nvarchar(100) not null,
    Email        nvarchar(150) not null unique,
    PhoneNumber  nvarchar(20)  null,
    JoinDate     datetime      not null default getdate(),
    IsActive     bit           not null ,

);

create table BorrowRecords (

    Id           int primary key identity(1,1),
    BorrowDate   datetime not null default getdate(),
    DueDate      datetime not null,
    ReturnDate   datetime null,
    Status       bit      not null,   -- 0 = borrowed, 1 = returned

    MemberId     int      not null,
    BookId       int      not null,

    constraint fk_BorrowRecords_Members foreign key (MemberId) references Members(MemberId),
    constraint fk_BorrowRecords_Books foreign key (BookId)   references Books(BookId),

    constraint chk_BorrowRecords_Dates check (DueDate > BorrowDate and (ReturnDate is null or ReturnDate >= BorrowDate)),
    constraint chk_BorrowRecords_Status check ((Status = 0 and ReturnDate is null) or (Status = 1 and ReturnDate is not null))
);
