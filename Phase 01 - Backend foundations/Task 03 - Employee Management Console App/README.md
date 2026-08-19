# Employee Management System

## Project Overview

The **Employee Management System** is a console-based HR application designed to manage employee records for a small business. This application demonstrates core C# and .NET concepts including collections, searching, filtering, sorting, LINQ, and service-layer architecture.

The system allows HR personnel to efficiently manage employee data while maintaining data integrity through deactivation rather than deletion, simulating real-world HR systems.

## Features

### Core Functionality

1. **Add Employee** - Create new employee records with comprehensive validation
2. **Update Employee** - Modify employee details while preserving EmployeeId
3. **Deactivate Employee** - Mark employees as inactive while keeping records intact
4. **Search Employee** - Find employees by ID or name (with partial match support)
5. **Filter by Department** - View all active employees in a specific department
6. **Sort Employees** - Sort by salary (ascending/descending), hire date, or name
7. **View All Employees** - Display all employees in a formatted table
8. **Salary Reports** - Generate comprehensive payroll and employee statistics


## Architecture

The project follows a clean, layered architecture:

### Models (`Models/`)
- **Employee.cs**: Core employee data model with properties and ToString method

### Services (`Services/`)
- **EmployeeService.cs**: Manages employee CRUD operations, search, filter, and sort
- **EmployeeReportService.cs**: Generates salary reports and statistics

### UI (`UI/`)
- **ConsoleMenu.cs**: Handles all user interface and menu navigation

### Application Entry Point
- **Program.cs**: Initializes services and starts the application

## Project Structure

```
Task 03 - Employee Management Console App/
│
├── Models/
│   └── Employee.cs
│
├── Services/
│   ├── EmployeeService.cs
│   └── EmployeeReportService.cs
│
├── UI/
│   └── ConsoleMenu.cs
│
├── Program.cs
├── Task 03 - Employee Management Console App.csproj
└── README.md
```

## Seed Data

The application initializes with **12 employees** across multiple departments:

| ID      | Name          | Department | Position           | Salary | Status   |
| ------- | ------------- | ---------- | ------------------ | -----: | -------- |
| EMP-001 | Mohamed Ayman | IT         | Backend Developer  |  20000 | Active   |
| EMP-002 | Sara Adel     | HR         | HR Specialist      |  12000 | Active   |
| EMP-003 | Ahmed Tarek   | IT         | Junior Developer   |   9000 | Active   |
| EMP-004 | Omar Samir    | Sales      | Sales Executive    |  11000 | Active   |
| EMP-005 | Mariam Hassan | Finance    | Accountant         |  14000 | Active   |
| EMP-006 | Khaled Ali    | IT         | DevOps Trainee     |  10000 | Active   |
| EMP-007 | Nour Emad     | Marketing  | Content Specialist |   9500 | Active   |
| EMP-008 | Youssef Nabil | Sales      | Sales Manager      |  18000 | Inactive |
| EMP-009 | Dina Farouk   | HR         | Recruiter          |  10500 | Active   |
| EMP-010 | Hady Mahmoud  | IT         | QA Engineer        |  13000 | Active   |
| EMP-011 | Salma Taha    | Finance    | Finance Manager    |  26000 | Active   |
| EMP-012 | Ali Mostafa   | Support    | Support Agent      |   8000 | Active   |

## Features Documentation

### Add Employee
- Prompts for all required fields: name, email, department, position, salary, hire date
- Auto-generates unique EmployeeId in format EMP-XXX
- Validates all inputs before creation
- New employees are automatically set as Active

### Update Employee
- Search for employee by ID
- Allows updating: email, department, position, and salary
- **EmployeeId cannot be changed** (immutable identifier)
- Validates all updated values before persisting

### Deactivate Employee
- Marks employee as inactive (IsActive = false)
- **Critical**: Employee records remain in the system for historical purposes
- Deactivated employees are excluded from department filters by default
- Can still be viewed in all employees list

### Search Employee
- **By ID**: Exact match search for EmployeeId
- **By Name**: Partial, case-insensitive search
  - Example: "Ahmed" finds "Ahmed Tarek"
  - Example: "ahm" finds "Ahmed Tarek"

### Filter by Department
- Case-insensitive department filtering
- Shows only active employees by default
- Example: "IT", "it", or "It" all return the same results

### Sort Employees
- **Salary Ascending**: Lowest to highest
- **Salary Descending**: Highest to lowest
- **Hire Date Ascending**: Oldest to newest hire date
- **Hire Date Descending**: Newest to oldest hire date
- **Name**: Alphabetical order

### Salary Reports
Generates comprehensive statistics including:
- Average salary across all employees
- Total payroll
- Employee with highest salary
- Employee with lowest salary
- Employee count by department (with alphabetical sorting)
- Active vs. Inactive employee counts

### View All Employees
Displays all employees (active and inactive) in a formatted table showing:
- Employee ID
- Full Name
- Email
- Department
- Position
- Salary
- Hire Date
- Status (Active/Inactive)

## Validation Rules

### Employee ID
- Must be unique (automatically generated and validated)
- Immutable once created

### Salary
- Must be greater than zero
- Rejects zero and negative values
- Uses decimal type (not float or double) for precision

### Hire Date
- Cannot be in the future
- Must be a valid date

### Required Fields
- Full Name, Email, Department, Position cannot be empty
- String trimming applied to all text inputs

### Input Safety
- All numeric inputs validated with TryParse
- All date inputs validated with TryParse
- Application continues gracefully on invalid input

## LINQ Concepts Used

The project extensively demonstrates LINQ capabilities:

- **Where()**: Filtering active employees, case-insensitive searches
- **FirstOrDefault()**: Finding single employees by ID
- **Contains()**: Partial string matching for employee names
- **OrderBy() / OrderByDescending()**: Sorting by salary, hire date, name
- **Average()**: Calculating average salary
- **Sum()**: Calculating total payroll
- **MaxBy() / MinBy()**: Finding highest and lowest paid employees
- **GroupBy()**: Grouping employees by department
- **Count()**: Counting active/inactive employees
- **ToList()**: Creating new collections from queries

## OOP & Architecture

### Separation of Concerns
- **Models**: Pure data representation
- **Services**: Business logic (CRUD, search, filter, sort, aggregation)
- **UI**: User interaction and presentation
- **Program**: Application initialization only

### Key Design Patterns
- **Service Layer Pattern**: Business logic isolated from UI
- **Dependency Injection**: Services injected into ConsoleMenu
- **Collection Management**: Internal list protected, methods return new Lists
- **Validation**: Centralized at service layer with meaningful error messages

### SOLID Principles Applied
- **Single Responsibility**: Each class has one reason to change
- **Dependency Inversion**: ConsoleMenu depends on service abstractions
- **Open/Closed**: Easy to extend with new reports or operations

## How to Run

### Steps
1. Navigate to the project directory
2. Run: `dotnet run`
3. The application starts with the main menu
4. Follow on-screen prompts for each operation
5. Select option 9 to exit

### Example Command
```bash
cd "Task 03 - Employee Management Console App"
dotnet run
```

## Testing

### Tested Scenarios

#### Add Employee
- ✓ Valid employee creation
- ✓ Empty name rejection
- ✓ Negative salary rejection
- ✓ Zero salary rejection
- ✓ Future hire date rejection
- ✓ Invalid date format handling
- ✓ Unique ID generation

#### Update Employee
- ✓ Existing employee update
- ✓ Missing employee error handling
- ✓ Email update
- ✓ Department update
- ✓ Position update
- ✓ Salary update with validation
- ✓ EmployeeId remains unchanged

#### Deactivate Employee
- ✓ Existing employee deactivation
- ✓ Missing employee error handling
- ✓ Employee remains in collection
- ✓ IsActive changes to false

#### Search
- ✓ Exact ID match
- ✓ Full name search
- ✓ Partial name search
- ✓ Case-insensitive search (Ahmed, ahmed, AHMED)
- ✓ No results handling

#### Department Filter
- ✓ Case-insensitive filtering (IT, it, It)
- ✓ Active employees only by default
- ✓ Existing department
- ✓ Non-existing department handling

#### Sorting
- ✓ Salary ascending
- ✓ Salary descending
- ✓ Hire date ascending
- ✓ Hire date descending
- ✓ Name alphabetical sorting

#### Reports
- ✓ Average salary calculation
- ✓ Highest salary identification
- ✓ Lowest salary identification
- ✓ Total payroll calculation
- ✓ Department count accuracy
- ✓ Active/inactive count accuracy


