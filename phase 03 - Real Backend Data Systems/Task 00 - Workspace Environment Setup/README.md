# Task 00 — Configuration, Secrets & Workspace Setup

## Overview

This task prepares the **Phase 03 — Real Backend Data Systems** workspace before implementing the database layer and backend functionality.

The objective is to establish a clean project structure, install the required Entity Framework Core packages, configure a local SQL Server connection safely, and define clear rules for handling secrets and production configuration.

> **Important:** Connection strings, passwords, API keys, and other sensitive production credentials must never be committed to the repository.

---

## Objectives

By completing this task, the project should have:

- A dedicated Phase 03 workspace.
- The required backend project structure.
- Entity Framework Core configured for SQL Server.
- EF Core CLI tooling available for migrations.
- A local development connection string.
- A safe `appsettings.json` configuration.
- Clear documentation for local database setup.
- No production credentials exposed in source control.
- An initial Git commit containing the Phase 03 structure.

---

## 1. Workspace Setup

Create the Phase 03 workspace using the following structure:

```text
phase-03-real-backend-data-systems/
└── TrainingCenter.Api/
    ├── Controllers/
    │   └── TestController.cs
    ├── Data/
    │   └── ApplicationDbContext.cs
    ├── Entities/
    │   └── Student.cs
    ├── DTOs/
    │   ├── Students/
    │   ├── Tracks/
    │   ├── Enrollments/
    │   ├── Payments/
    │   └── Reports/
    ├── Services/
    │   └── TestService.cs
    │   └── ITestService.cs
    ├── Utilities/
    │   ├── ApiResponse.cs
    │   └── PaginationResult.cs
    ├── Program.cs
    └── appsettings.json
```

### Folder Responsibilities

| Folder | Responsibility |
|---|---|
| `Controllers/` | HTTP endpoints and request handling |
| `Data/` | Database context and data-access configuration |
| `Entities/` | Database/domain entities |
| `DTOs/` | Request and response models |
| `DTOs/Students/` | Student-related DTOs |
| `DTOs/Tracks/` | Track-related DTOs |
| `DTOs/Enrollments/` | Enrollment-related DTOs |
| `DTOs/Payments/` | Payment-related DTOs |
| `DTOs/Reports/` | Reporting-related DTOs |
| `Services/` | Business logic and application services |
| `Common/` | Shared response and pagination models |
| `Program.cs` | Application startup and dependency injection configuration |
| `appsettings.json` | Local development configuration |

---

## 2. Required Packages

Install the following Entity Framework Core packages:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```

These packages provide:

- **`Microsoft.EntityFrameworkCore.SqlServer`** — SQL Server provider for EF Core.
- **`Microsoft.EntityFrameworkCore.Tools`** — EF Core tooling support.
- **`Microsoft.EntityFrameworkCore.Design`** — design-time services required for migrations and related tooling.

### EF Core CLI

If `dotnet-ef` is not installed globally:

```bash
dotnet tool install --global dotnet-ef
```

Verify the installation:

```bash
dotnet ef --version
```

---

## 3. Development Connection String

For local development, configure the SQL Server connection in:

```text
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TechMasterTrainingCenterDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Important

This example uses Windows Authentication through:

```text
Trusted_Connection=True
```

If SQL Server authentication is used instead, credentials should be handled locally and securely rather than committing real passwords to Git.

For example, do **not** commit:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MyDb;User Id=admin;Password=RealPassword123;"
  }
}
```

---

## 4. Configuration & Secrets Rules

Professional backend projects must separate **configuration** from **secrets**.

### Development Configuration

Local development connection strings may be stored in a development-specific configuration file when appropriate.

Example:

```text
appsettings.json
```

However, real production credentials must not be pushed to GitHub.

### Production Configuration

Production connection strings should be configured through the hosting provider's environment/configuration panel or another secure secret-management mechanism.

The production connection string should **not** be stored in this repository.

Recommended documentation:

```text
Production connection string:
Configured through the hosting provider's configuration/environment settings.
It is not stored in this repository.
```


## 5. Recommended README Setup Section

The following section can be included in the project's main README:

```md
## Phase 03 Local Setup

1. Install SQL Server or use a remote SQL Server database.
2. Update `ConnectionStrings:DefaultConnection` locally.
3. Run:
   `dotnet ef migrations add InitialTrainingCenterSchema`
4. Run:
   `dotnet ef database update`
5. Run the API and open Swagger.

> Production connection strings are configured through the hosting provider's
> configuration/environment panel and are not stored in this repository.
```

---

## 6. Database Preparation

Once the project and configuration are ready, verify that EF Core can communicate with the database.

### Create the Initial Migration

```bash
dotnet ef migrations add InitialTrainingCenterSchema
```

This creates the initial database schema migration.

### Apply the Migration

```bash
dotnet ef database update
```

This applies the migration to the configured SQL Server database.

### Run the API

```bash
dotnet run
```

Then open the Swagger UI exposed by the application.

---