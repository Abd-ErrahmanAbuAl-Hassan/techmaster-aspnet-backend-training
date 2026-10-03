CREATE DATABASE TrainingCenterDB;

USE TrainingCenterDB;

create table Students (
    StudentId    int primary key identity(1,1),
    FullName     nvarchar(100) not null,
    Email        nvarchar(100) not null unique,
    PhoneNumber  nvarchar(20),
    CreatedAt    datetime      not null default GETDATE()
);

create table Instructors (
    InstructorId    int primary key identity(1,1),
    FullName        nvarchar(100) not null,
    Email           nvarchar(100) not null unique,
    Specialization  nvarchar(100)
);

create table Tracks (
    TrackId        int primary key identity(1,1),
    Title          nvarchar(100) not null,
    Description    nvarchar(300),
    DurationWeeks  int           not null check (DurationWeeks > 0),
    StartDate      date          not null,
    InstructorId   int           not null,

    constraint FK_Tracks_Instructors foreign key (InstructorId) references Instructors(InstructorId)
);

create table Registrations (
    RegistrationId    int primary key identity(1,1),
    StudentId         int           not null,
    TrackId           int           not null,
    RegistrationDate  datetime      not null default GETDATE(),
    Status            nvarchar(20)  not null default N'Pending'
                      check (Status IN (N'Pending', N'Confirmed', N'Cancelled', N'Completed')),

    constraint FK_Registrations_Students foreign key (StudentId) references Students(StudentId),
    constraint FK_Registrations_Tracks foreign key (TrackId)   references Tracks(TrackId),
);

create table Payments (
    PaymentId      int primary key identity(1,1),
    RegistrationId int            not null unique, 
    Amount         decimal(10,2)  not null check (Amount >= 0),
    PaymentDate    datetime       not null default GETDATE(),
    PaymentStatus  nvarchar(20)   not null default N'Pending'
                   check (PaymentStatus IN (N'Pending', N'Paid', N'Failed', N'Refunded')),

    constraint FK_Payments_Registrations foreign key (RegistrationId) references Registrations(RegistrationId)
        
);


