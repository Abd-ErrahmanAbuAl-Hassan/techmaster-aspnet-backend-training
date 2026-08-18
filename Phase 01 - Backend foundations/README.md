# Phase 01 — Backend Foundations

This phase contains foundational backend exercises and small projects
designed to build confidence with C#, .NET, and core programming
concepts. Use the task folders for hands-on practice; each task has a
README with run instructions and expected results.

---

## Goals

- Establish solid C# fundamentals
- Practice input validation and defensive coding
- Learn idiomatic patterns and small-scale project structure
- Produce clear, maintainable console applications

---

## Tasks in this phase

- `Task 01 - csharp-drills` — Small logic drills (temperature converter,
  parsing, string problems)
- (Additional tasks may be added: bank account, employee management,
  product catalog, debugging/refactor exercises)

---

## How to use this phase

1. Open the phase folder and choose a task, for example:

```powershell
cd "Phase 01 - Backend foundations/Task 01 - csharp-drills"
```

2. Follow the task README and run using the .NET CLI:

```powershell
dotnet run --project "Task 01 - csharp-drills.csproj"
```

3. Implement, test, and optionally submit a PR for review.

---

## Contribution & Standards

- Keep solutions small and single-purpose.
- Validate console input and provide helpful error messages.
- Add basic unit tests where practical.
- Follow common C# naming and style conventions.

When contributing, include a short description and example output.

---

---
## Task 01 - Drills

| Drill No. | Drill Name | Topic | Status | Notes |
|---|---|---|---|---|
| 01 | Temperature Converter | Parsing / Calculation | Done | Handles invalid input |
| 02 | Grade Calculator | Conditions | Done | Validates score range (0-100), error handling for invalid input |
| 03 | Login Validator | Loops / Strings | Done | Max 3 attempts, username/password validation |
| 04 | Even-Odd Analyzer | Conditions | Done | Analyzes even and odd numbers |
| 05 | Maximum and Minimum Finder | Comparison | Done | Finds max/min values in a collection |
| 06 | Word Counter | Strings | Done | Counts word occurrences in text |
| 07 | Name Formatter | String Manipulation | Done | Formats names properly |
| 08 | Password Strength Checker | Validation | Done | Checks length, uppercase, lowercase, digits, special characters |
| 09 | Shopping Cart Total | Calculation | Done | Calculates total with multiple items |
| 10 | Simple ATM Menu | Loops / Conditions | Done | Menu-driven interface with balance, withdraw, deposit operations |
| 11 | Duplicate Number Detector | Arrays | Done | Identifies duplicate numbers in arrays |
| 12 | Email Validator | String Validation | Done | Validates email format |
| 13 | Palindrome Checker | Strings / Logic | Done | Checks if strings are palindromes |
| 14 | Simple Expense Tracker | Data Structure | Done | Tracks and manages expenses |
| 15 | Array Rotation | Arrays | Done | Rotates array elements |
| 16 | Frequency Counter | Arrays | Done | Counts frequency of numbers with error validation |
| 17 | Simple Search Engine | Arrays / Strings | Done | Searches through arrays/strings |
| 18 | Number Statistics | Arrays / Math | Done | Calculates statistics for numbers |
| 19 | Simple Ticket Price Calculator | Conditions / Calculation | Done | Calculates prices based on conditions |
| 20 | Method Refactoring Challenge | Code Organization | Done | Refactors code for better organization |

---

## Required Screenshots

### Grade Calculator
Demonstrates score input validation (0-100 range), error handling for invalid input, and grade assignment based on score boundaries.

![Grade Calculator - Valid Score](../screenshots/Screenshot%202026-08-18%20183934.jpg)

![Grade Calculator - Score Below Range](../screenshots/Screenshot%202026-08-18%20184003.jpg)

![Grade Calculator - Score Above Range](../screenshots/Screenshot%202026-08-18%20184111.jpg)

![Grade Calculator - Invalid Input](../screenshots/Screenshot%202026-08-18%20184140.jpg)

### Login Validator
Shows username and password validation, failed login attempts tracking, account lock after 3 failed attempts, and successful login for valid credentials.

![Login Validator - Successful Login](../screenshots/Screenshot%202026-08-18%20184416.jpg)

![Login Validator - Failed Attempts](../screenshots/Screenshot%202026-08-18%20184534.jpg)

![Login Validator - Account Locked](../screenshots/Screenshot%202026-08-18%20184622.jpg)

### Simple ATM Menu
Menu-driven interface with 4 options (Check Balance, Withdraw, Deposit, Exit). Shows balance checking, withdrawal with error handling, deposit functionality, and error validation.

![ATM Menu - Check Balance](../screenshots/Screenshot%202026-08-18%20184709.jpg)

![ATM Menu - Insufficient Balance Error](../screenshots/Screenshot%202026-08-18%20184740.jpg)

![ATM Menu - Successful Withdrawal](../screenshots/Screenshot%202026-08-18%20184757.jpg)

![ATM Menu - Invalid Deposit Amount](../screenshots/Screenshot%202026-08-18%20184817.jpg)

![ATM Menu - Successful Deposit](../screenshots/Screenshot%202026-08-18%20184841.jpg)

### Password Strength Checker
Validates password length (minimum 8 characters), checks for uppercase letters, lowercase letters, digits, and special characters. Shows weak vs. strong password indicators.

![Password Strength Checker - Weak Password](../screenshots/Screenshot%202026-08-18%20184947.jpg)

![Password Strength Checker - Strong Password](../screenshots/Screenshot%202026-08-18%20185017.jpg)

![Password Strength Checker - Missing Special Character](../screenshots/Screenshot%202026-08-18%20185043.jpg)


### Frequency Counter
Accepts space-separated list of numbers, shows error handling for invalid input, displays frequency count of each unique number, and demonstrates input validation and error recovery.

![Frequency Counter - Empty Input Error](../screenshots/Screenshot%202026-08-18%20185135.jpg)

![Frequency Counter - Invalid Number Error](../screenshots/Screenshot%202026-08-18%20185209.jpg)

---
