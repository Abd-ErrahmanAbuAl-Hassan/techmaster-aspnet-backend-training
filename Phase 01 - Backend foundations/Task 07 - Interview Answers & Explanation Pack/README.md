# Task 07 - Interview Answers & Explanation Pack

## 📖 Overview
This repository contains the answers and explanations for Task 07. The goal of this task is to demonstrate the ability to explain technical concepts clearly, a crucial skill for professional software development.

**Objective:** Explain what was built, not just how to build it.
---

## 🎯 Required Questions & Answers

### 1. What is the difference between class and object?
**Answer:**
A **Class** is a blueprint or template that defines the properties and behaviors of a concept. An **Object** is a specific instance of that class created.

**Example:**
```csharp
class BankAccount{
    private string _accountNumber;

    public string AccountNumber => _accountNumber;
    public decimal Balance {get; private set;}

    public void Deposit(decimal amount){
        Balance += amount;
    }

    public void Withdraw(decimal amount){
        Balance -= amount;
    }
}

BankAccount account = new BankAccount(); // account is an object of a BankAccount class.
```
---

### 2. What is encapsulation?
**Answer:**
Encapsulation is the concept of wrapping fields and methods together as a single unit and restricting direct access to the data from outside the class. It is usually achieved using private fields and public properties/methods.

**Example:**
A `BankAccount` class hides the `Balance` property (private set) and only allows changes via `Deposit()` or `Withdraw()` methods.

---

### 3. Why should account balance not be public?
**Answer:**
If the balance were public, any part of the program could change it to an arbitrary value (e.g., setting a balance to -$1,000,000) without validation or logging. Keeping it private ensures data integrity.

**Example:**
```csharp
// Bad
public decimal Balance; 

// Good
private decimal _balance; 
public decimal Balance => _balance;
```
---

### 4. What is the difference between field and property?
**Answer:**
A Field is a variable declared directly in a class. A Property is a member that provides a flexible mechanism to read, write, or compute the value of a private field (using get and set accessors).

**Example:**

```csharp
private decimal _balance; // Field
public decimal Balance {get; private set;} // Property
```
---

### 5. Why do we use constructors?
**Answer:**
Constructors are used to initialize an object when it is created. They ensure that the object starts in a valid state with all necessary data provided.

**Example:**

```csharp
 public class EmployeeReportService
 {
    private readonly EmployeeService _employeeService;

    //ctor
    public EmployeeReportService(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }
}
```
---

### 6. What is the purpose of a service class?
**Answer:**
A service class encapsulates business logic and separates it from the controller.
It acts as a middle layer between the UI/Controller and the Data Access layer.

**Example:**
An EmployeeService class that handles creating employee, rather than putting that logic inside the Employee class or in UI/Controller.

---

### 7. Why should we avoid huge Main methods?
**Answer:**
We should avoid huge Main methods because they make code harder to understand, maintain, debug, and unit test.
A better approach is to separate responsibilities into methods or classes. 

---

### 8. What is the difference between List and Array?
**Answer:**
An array has a fixed size that is determined when it is created, while a List is dynamically resizable and allows us to add or remove elements during runtime.

**Example:**
```csharp
int[] numbers = new int[5]; //Fixed size
List<int> numbers = new List<int>(); //Dynamic size
```
---
### 9. When would you use Dictionary?
**Answer:**
A Dictionary stores data as key-value pairs and uses hashing to locate values by their keys. It provides average O(1) lookup time, which makes it useful when we frequently need to retrieve a value using a unique key. In comparison, searching for an element in a list by its value typically takes O(n) time.
---

### 10. What is LINQ used for?
**Answer:**
LINQ => Language Integrated Query. 
It provides a consistent way to query and manipulate data from different sources, such as collections and databases. 
It offers operations like filtering, projection, and grouping, which make code more readable, maintainable, and easier to express than writing the same logic manually.

**Example:**
```csharp
var totalEmployeesSalaries = employees.Sum(e => e.Salary);
```
---
### 11. What is the difference between Where and Select?
**Answer:**
Where is used to filter a collection based on a condition and returns the elements that satisfy it. Select projects each element into a new form.

**Example:**
```csharp
 var employees = _employeeService.GetAllEmployees()
    .Where(e => e.Department.Equals("HR", StringComparison.OrdinalIgnoreCase))
    .Select(e=>new {e.Name , e.Email})
    .ToList();
```
---
### 12. What is GroupBy used for?
**Answer:**
`GroupBy` groups elements based on one or more keys and returns a collection of groups. Each group has a key and contains the elements that share that key. We can then apply operations such as `Sum` or `Count` to each group to analyze the data.

**Example:**
```csharp
return employees
    .GroupBy(e => e.Department)
    .OrderBy(g => g.Key)
    .ToDictionary(g => g.Key, g => g.Count());
```
---
### 13. What are Skip and Take used for?
**Answer:**
They are used for Pagination. Skip(n) bypasses the first n elements, and Take(n) returns the next n elements.

**Example:**
```csharp
return employees.Skip((pageNumber - 1)*pageSize).Take(pageSize).ToList();
```
---

### 14. What is a primary key?
**Answer:**
A primary key is a column or set of columns in a database table that uniquely identifies each row. It cannot be null and must be unique.
---

### 15. What is a foreign key?
**Answer:**
A foreign key is a column that creates a link between two tables. It refers to the primary key of another table, enforcing referential integrity.
---

### 16. What is a one-to-many relationship?
**Answer:**
A relationship where one record in Table A relates to many records in Table B, but a record in Table B relates to only one record in Table A.
---

### 17. Why do we use JOIN?
**Answer:**
We use JOIN to combine rows from two or more tables based on a related column between them. It allows us to retrieve data spread across normalized tables in a single query.

**Example:**
```sql
SELECT b.* FROM Books b INNER JOIN Categories c 
ON b.CategoryId = c.CategoryId
WHERE c.Name = N'Fantasy';
```
---
### 18. What is the difference between table and entity?
**Answer:**
A Table is a physical database structure (rows and columns). An Entity is a C# class in code that represents that table (ORM mapping).
---
### 19. Why do we use GitHub?
**Answer:**
GitHub is a cloud-based platform for version control using Git. It allows for collaboration, code backup, tracking changes, and managing project workflows.
---
### 20. What makes a README useful?
**Answer:**
A README explains what the project is, how to install it, and how to run it. It is the entry point for anyone looking at the code.
---

### 21. Why are multiple commits better than one final commit?
**Answer:**
Multiple commits provide a detailed history of changes. If a bug is introduced, it's easier to find which change caused it. It also allows for better collaboration and code reviews.
---

### 22. Why is professional delivery important?
**Answer:**
Professional delivery ensures the code is maintainable, scalable, and readable by others. It shows respect for the team and the client, reducing technical debt.