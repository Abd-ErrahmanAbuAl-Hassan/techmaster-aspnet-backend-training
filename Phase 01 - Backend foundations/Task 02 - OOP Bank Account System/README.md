# Bank System

A professional console-based banking system demonstrating enterprise-grade OOP design, clean architecture, and domain-driven development principles.

**Core Operations:**
- ✅ Create customer accounts with validation
- ✅ Deposit & withdraw money with automatic transaction recording
- ✅ Transfer money between accounts (atomic operations)
- ✅ View account details and transaction history
- ✅ Display all accounts with formatted summary

**Quality Attributes:**
- Encapsulated balance protection (impossible to create invalid states)
- Complete transaction audit trail
- Comprehensive multi-layer validation
- Meaningful error messages
- Professional formatted console UI

---

## Architecture

**Layered Design:**

### Layer Responsibilities

| Layer | Responsibility | Files |
|-------|-----------------|-------|
| **UI** | Display, input, output only | `ConsoleMenu.cs` |
| **Service** | Business logic, orchestration, validation | `BankService.cs` |
| **Domain** | State protection, business rules | `Models/` |
| **Entry** | Application startup | `Program.cs` |

### Domain Models

- `Customer` - Customer data with validation
- `BankAccount` - Account state + **encapsulated balance**
- `Transaction` - Immutable transaction records
- `AccountType` (enum) - Checking, Savings, Business, StudentSavings
- `TransactionType` (enum) - Deposit, Withdraw, TransferOut, TransferIn

---

## Key Design: Encapsulated Balance

**Problem:** Without protection, invalid states are possible:

**Solution:** Balance only modifiable through controlled domain methods:

**Result:** ✅ Impossible to create invalid states

---

## Project Structure

---

## Usage Example
========================================
           BANK SYSTEM
========================================

====== MAIN MENU ======
1.	Create Customer Account
2.	Deposit Money
3.	Withdraw Money
4.	Transfer Money
5.	View Account Details
6.	View Transaction History
7.	View All Accounts
8.	Exit
Choose an option (1-8): 1 

====== CREATE NEW ACCOUNT ====== 

Enter full name: Abdulrahman Mohamed
Enter email: aboda@gmail.com 
Enter phone number: 01012341234 
Enter initial balance: 5000
Select Account Type:
1.	Checking
2.	Savings
3.	Business
4.	StudentSavings Enter choice (1-4): 1
 
Account created successfully! 
Account Number: ACC-876 
Account Type: Checking 
Balance: $5,000.00 
Customer: Abdulrahman Mohamed



---

## Business Rules

### Account Creation
- Customer name, email, and phone number are required
- Initial balance cannot be negative
- Account numbers are automatically generated and unique

### Deposits
- Amount must be positive (> 0)
- Creates automatic transaction record
- Account must be active

### Withdrawals
- Amount must be positive (> 0)
- Balance must be sufficient (no overdraft)
- Creates automatic transaction record
- Account must be active

### Transfers
- Source and destination must be different accounts
- Amount must be positive (> 0)
- Source must have sufficient balance
- **Atomic operation:** Both succeed or both fail
- Creates `TransferOut` transaction (source)
- Creates `TransferIn` transaction (destination)

### Financial Data
- All money stored as `decimal` 
- Balance can never be negative
- Transactions are immutable
- Transaction history sorted by date (newest first)

---

## Validation Strategy

**Multi-layer validation (each layer owns its responsibility):**

1. **UI Layer** (ConsoleMenu)
   - Parses input: `int.TryParse()`, `decimal.TryParse()`
   - Displays errors to user

2. **Service Layer** (BankService)
   - Enforces business rules (account exists, sufficient balance)
   - Coordinates operations

3. **Domain Layer** (Models)
   - Protects state (balance constraints)
   - Throws exceptions for invalid operations

**Result:** No duplicate validation; each layer has single responsibility.

---

## OOP Concepts Demonstrated

### 1. Encapsulation ⭐
Private balance field with read-only property. Balance only changes through controlled methods.

### 2. Abstraction
Services abstract complexity. UI doesn't know how transactions are created or how transfer atomicity works.

### 3. Composition
BankAccount *has-a* Customer (not inheritance). Account *has-a* list of Transactions.

### 4. Single Responsibility
Each class has one reason to change:
- `Customer` - customer data changes
- `BankAccount` - account behavior changes
- `Transaction` - transaction structure changes
- `BankService` - business logic changes
- `ConsoleMenu` - UI changes

### 5. Immutability
`Transaction` objects are immutable once created (no setters). Guarantees accurate audit trail.

### 6. Deliberate Design Decisions
- **No inheritance** - Composition is simpler and more flexible
- **No interfaces** - Single implementations don't need abstraction
- **Decimal for money** - Exact arithmetic (never float/double)

---

## Error Handling

**Invalid Scenarios Handled:**

Empty name → Error: Name cannot be empty. 
Negative balance → Error: Initial balance cannot be negative. 
Account not found → Error: Account was not found. 
Insufficient funds → Error: Insufficient balance. 
Same source/dest → Error: Source and destination accounts must be different. 
Invalid numeric input → Error: Invalid input.


**Strategy:** Never crash from user input. Always display helpful error message.

---

## Testing

All scenarios verified:

✅ **Account Creation** - Valid/invalid names, emails, phones, balances
✅ **Deposits** - Valid/invalid amounts, missing accounts
✅ **Withdrawals** - Valid/invalid amounts, insufficient funds
✅ **Transfers** - Valid/invalid source/destination, atomicity
✅ **Queries** - Account details, transaction history, all accounts
✅ **Error Handling** - All invalid scenarios caught gracefully

---

## Code Quality

**Standards Followed:**
- PascalCase for types, methods, properties
- camelCase for local variables and parameters
- Meaningful, self-documenting names
- Small, focused methods
- Proper access modifiers
- No magic numbers or commented-out code
- Comprehensive XML documentation

**SOLID Principles:**
- **S**ingle Responsibility - Each class has one reason to change
- **O**pen/Closed - Extensible via enums (new account types)
- **L**iskov Substitution - Not violated (no inheritance hierarchies)
- **I**nterface Segregation - Minimal public surfaces
- **D**ependency Inversion - UI depends on Service, not reverse

---

## Design Highlights

### Why Encapsulated Balance?
Ensures business rules are always enforced. Impossible to create negative balance or skip transaction recording.

### Why Atomic Transfers?
Prevents partial transfers. Either both accounts update or neither does. System never left in inconsistent state.

### Why Multi-Layer Validation?
- UI validates input format (not business logic)
- Service validates business rules (not display logic)
- Domain validates state constraints (not caller concerns)

Result: Clear separation of concerns, no duplication, maintainable code.

### Why Decimal for Money?
Floating-point arithmetic has precision errors: `0.1 + 0.2 ≠ 0.3`. Decimal uses base-10, is exact for financial values.


## Summary

This is production-grade code demonstrating:
- ✅ Professional OOP design
- ✅ Clean architecture with clear separation of concerns
- ✅ Encapsulation as a security mechanism
- ✅ Atomic operations and transaction management
- ✅ Comprehensive error handling
- ✅ Code quality and maintainability

**Ready for senior technical review and interview discussion.**
