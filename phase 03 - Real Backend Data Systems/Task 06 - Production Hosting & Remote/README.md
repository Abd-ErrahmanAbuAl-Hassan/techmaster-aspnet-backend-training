# TechMaster Academy — Backend API

> A production-grade ASP.NET Core Web API for a training center platform, covering students, instructors, training tracks, enrollments, payments, and reporting — with enforced business rules, audit trails, soft delete, and Azure deployment.

---

## 🔗 Live Demo

| Resource | URL |
|----------|-----|
| **Live Swagger UI** | https://techmasterapi.azurewebsites.net/swagger/index.html |
| **Health Check** | https://techmasterapi.azurewebsites.net/health |
| **Sample GET (Instructors)** | https://techmasterapi.azurewebsites.net/api/instructors |
| **GitHub Repository** | https://github.com/Abd-ErrahmanAbuAl-Hassan/techmaster-aspnet-backend-training.git |

> The API is hosted on Azure App Service (Linux, .NET 8) and connects to Azure SQL Database. It runs in the **Staging** environment so the Swagger UI is publicly accessible for evaluation.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Domain Model](#domain-model)
- [Business Rules](#business-rules)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [Local Setup](#local-setup)
- [Production Deployment (Azure)](#production-deployment-azure)
- [Configuration & Secrets](#configuration--secrets)
- [Database Migrations & Seeding](#database-migrations--seeding)
- [Deployment Evidence](#deployment-evidence)
- [Demo Video](#demo-video)
- [Known Limitations](#known-limitations)
- [Author](#author)

---

## Overview

**TechMaster Academy** is a backend API for managing a training center's operations:

- **Students** enroll in **Training Tracks** taught by **Instructors**
- **Enrollments** move through a controlled lifecycle (`Draft → Active → Completed` or `Cancelled`)
- **Payments** are cumulative — an enrollment becomes `Active` once payments cover the track price
- **Refunds** are recorded as separate payment rows when an enrollment is cancelled
- **Reports** aggregate revenue, unpaid amounts, capacity, and instructor workload

The project emphasizes **business rules and data integrity** at three layers: application logic, EF Core configuration, and database constraints.

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Azure App Service (Linux)                │
│  ┌───────────────────────────────────────────────────────┐  │
│  │            ASP.NET Core 8 Web API                     │  │
│  │  Controllers → Services → EF Core → DbContext         │  │
│  └───────────────────────────────────────────────────────┘  │
└──────────────────────────┬──────────────────────────────────┘
                           │ SQL Authentication (TLS)
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                    Azure SQL Database                       │
│               TechMasterTrainingCenterDb                    │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Students │ Instructors │ Tracks │ Enrollments │ ...  │  │
│  │  + Check constraints, unique indexes, FKs             │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| Runtime | .NET 8 (LTS) |
| Web Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 |
| Database | Azure SQL Database |
| Hosting | Azure App Service (Linux, Free/Basic tier) |
| Documentation | Swagger / OpenAPI |
| Logging | ASP.NET Core built-in + App Service Log Stream |
| Deployment | Local Git → Kudu → Oryx build |

---

## Domain Model

### Entities

| Entity | Purpose |
|--------|---------|
| `Person` (abstract) | Shared fields for `Student` and `Instructor` — name, email, phone, audit |
| `Student` | Learner; implements `ISoftDelete` |
| `Instructor` | Teacher; owns training tracks |
| `TrainingTrack` | Course with capacity, price, schedule, status; implements `ISoftDelete` |
| `Enrollment` | Student ↔ Track link with status, progress, grade |
| `Payment` | Cumulative payment against an enrollment; supports refund rows |

### Interfaces

- **`IAudiable`** — `CreatedAt`, `UpdatedAt` (auto-stamped in `SaveChangesAsync`)
- **`ISoftDelete`** — `IsDeleted`, `DeletedAt` (rows are never physically deleted)

### Enums

- `EnrollmentStatus` — `Draft`, `Active`, `Completed`, `Cancelled`
- `PaymentStatus` — `Pending`, `PartiallyPaid`, `Paid`, `Refunded`, `Failed`
- `PaymentMethod` — `VodafoneCash`, `Instapay`, `Fawry`, `ApplePay`, `CreditCard`, `Cash`, `BankTransfer`
- `TrackLevel` — `Beginner`, `Intermediate`, `Advanced`
- `TrackStatus` — `Pending`, `Published`, `Archived`, `Draft`, `Closed`

---

## Business Rules

### Enrollment
- A student cannot enroll in the same track twice while a non-cancelled enrollment exists
- Enrollment is blocked if the track is `Closed` or `Archived`
- Enrollment is blocked if the track capacity is full (`Active + Completed` counts as enrolled)
- Status transitions are validated:
  - `Draft → Active`
  - `Draft → Cancelled`
  - `Active → Completed`
- Cancelling a paid enrollment creates a **refund payment row**

### Payment
- Payments can only be created for non-cancelled enrollments
- Payments cannot exceed the track price
- A payment is marked `Paid` if it is the last installment that completes the total; otherwise `PartiallyPaid`
- A fully-paid enrollment automatically transitions from `Draft → Active`
- Refunds are modeled as new payment rows with `PaymentStatus.Refunded`

### Training Track
- `Price > 0`, `Capacity > 0`, `EndDate > StartDate` (enforced via DB check constraints)
- Cannot soft-delete a track with active enrollments
- Unique track `Code` (8 chars)

### Student / Instructor
- Unique email and phone (filtered to exclude soft-deleted rows)
- Soft delete supported for students; students are hidden from default queries

### Reports
- **Total Revenue** = sum of `Paid` + `PartiallyPaid` payments − `Refunded` payments
- **Unpaid Amount** = sum over `Draft` enrollments of `max(0, TrackPrice − AmountCollected)`
- Capacity reports count `Active + Completed` as "enrolled"

---

## Project Structure

```
Task 05 - Business Rules & Data Integrity/
├── Controllers/           # API endpoints
├── Services/              # Business logic
│   └── Interfaces/        # Service contracts
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Seed/DbSeeder.cs
├── DTOs/
│   ├── Requests/          # Inbound payloads
│   └── Responses/         # Outbound shapes
├── Entities/              # EF Core entities
├── Utilities/
│   ├── Enums/             # Enum definitions
│   ├── Interfaces/        # IAudiable, ISoftDelete
│   ├── ApiResponse.cs
│   ├── PagedResult.cs
│   └── PaymentCalculator.cs
├── Migrations/            # EF Core migrations
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Production.json
└── TechMasterAPI.csproj
```

---

## API Endpoints

### Students — `/api/students`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/students` | Paged list with filters (search, isActive, isDeleted) |
| GET | `/api/students/{id}` | Student details with enrollments |
| POST | `/api/students` | Create student |
| PUT | `/api/students/{id}` | Update student |
| DELETE | `/api/students/{id}` | Soft delete |

### Instructors — `/api/instructors`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/instructors` | Paged list |
| GET | `/api/instructors/{id}` | Details |
| GET | `/api/instructors/{id}/tracks` | Instructor's tracks |
| POST | `/api/instructors` | Create |
| PUT | `/api/instructors/{id}` | Update |

### Training Tracks — `/api/tracks`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/tracks` | Paged list with filters |
| GET | `/api/tracks/{id}` | Details |
| POST | `/api/tracks` | Create (auto-generates code) |
| PUT | `/api/tracks/{id}` | Update |
| DELETE | `/api/tracks/{id}` | Soft delete (blocked if active enrollments) |

### Enrollments — `/api/enrollments`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/enrollments` | Paged list with filters |
| GET | `/api/enrollments/{id}` | Details |
| POST | `/api/enrollments` | Create |
| PUT | `/api/enrollments/{id}/status` | Update status (with transition rules + auto-refund) |
| GET | `/api/enrollments/student/{studentId}` | Student's enrollments |
| GET | `/api/enrollments/track/{trackId}/students` | Track's students |

### Payments — `/api/payments`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/payments` | Paged list with date/status filters |
| POST | `/api/payments` | Create payment |
| GET | `/api/payments/enrollment/{enrollmentId}` | Enrollment's payments |
| PUT | `/api/payments/{id}/status` | Update status (with transition rules) |

### Reports — `/api/reports`
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/reports/dashboard-summary` | KPI overview |
| GET | `/api/reports/unpaid-enrollments` | Draft enrollments with outstanding balance |
| GET | `/api/reports/track-capacity` | Capacity utilization per track |
| GET | `/api/reports/tracks-with-available-seats` | Tracks with seats remaining |
| GET | `/api/reports/revenue-summary` | Revenue breakdown |
| GET | `/api/reports/revenue-by-track` | Revenue per track |
| GET | `/api/reports/top-tracks` | Top N tracks by enrollment |
| GET | `/api/reports/instructors-workload` | Instructor load report |
| GET | `/api/reports/students-without-payments` | Students with no payments |

---

## Local Setup

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB, Express, or Developer Edition)
- Visual Studio 2022 or VS Code

### Steps

```bash
# 1. Clone
git clone https://github.com/Abd-ErrahmanAbuAl-Hassan/techmaster-aspnet-backend-training.git
cd "phase 03 - Real Backend Data Systems/Task 05 - Business Rules & Data Integrity"

# 2. Configure local connection string
#    Edit appsettings.Development.json → ConnectionStrings:DefaultConnection

# 3. Apply migrations
dotnet ef database update

# 4. Run
dotnet run
```

Swagger opens at `https://localhost:<port>/swagger`.

---

## Production Deployment (Azure)

### Infrastructure
| Resource | Configuration |
|----------|---------------|
| Azure SQL Server | `techmaster-sql.database.windows.net` |
| Azure SQL Database | `TechMasterTrainingCenterDb` — Basic tier |
| App Service Plan | Linux, Free (F1) or Basic (B1) |
| App Service | `TechMasterAPI` — .NET 8 runtime |

### Production DB Setup

The production database is **Azure SQL Database**, provisioned with SQL Authentication. The application connects using a **connection string stored in App Service configuration** — it is **never committed to source control**.

On first startup:
1. `Program.cs` calls `db.Database.MigrateAsync()` — creates the schema from EF Core migrations.
2. If `ASPNETCORE_ENVIRONMENT` is `Staging` or `Development`, the `DbSeeder` runs — inserting a test dataset for evaluation.
3. The API begins serving requests.

The **connection string is injected as an App Service Connection String** (`SQLAzure` type), so it overrides any local `appsettings.json` value. This keeps credentials out of the repository.

### Deployment Method

Local Git deployment via Kudu:

```bash
# Configure Azure as a Git remote
git remote add azure https://azAboda@techmasterapi.scm.azurewebsites.net/TechMasterAPI.git

# Deploy
git push azure main
```

Azure's Oryx build engine detects the `.csproj`, restores packages, publishes in Release mode, and swaps the running container.

---

## Configuration & Secrets

### What's Committed
- `appsettings.json` — non-sensitive defaults (logging, CORS placeholder)
- `appsettings.Development.json` — local connection string (LocalDB / Windows auth)
- `appsettings.Production.json` — environment-specific non-secrets (log level, CORS origins)

### What's NOT Committed
- **Production connection string** — stored in Azure App Service → Configuration → Connection Strings
- **Deployment credentials** — created via `az webapp deployment user set`
- **Any password** — never appears in source control, README, or logs

### Azure App Service Configuration
| Type | Name | Value |
|------|------|-------|
| Connection String (SQLAzure) | `DefaultConnection` | `Server=tcp:techmaster-sql.database.windows.net,1433;Initial Catalog=TechMasterTrainingCenterDb;...;Password=***;...` |
| Application Setting | `ASPNETCORE_ENVIRONMENT` | `Staging` |
| Application Setting | `DEPLOYMENT_BRANCH` | `main` |

Sensitive values are masked in Azure Portal and CLI output.

---

## Database Migrations 

### Migrations
Generated via:
```bash
dotnet ef migrations add <MigrationName>
```

Applied automatically on startup via `db.Database.MigrateAsync()`.

Migration history is tracked in the `__EFMigrationsHistory` table.

### Seeding
`DbSeeder.SeedAsync()` inserts a realistic dataset covering:
- 4 instructors (3 active, 1 inactive)
- 6 training tracks (5 published, 1 archived)
- 11 students (9 active, 1 inactive, 1 soft-deleted)
- 10 enrollments across all statuses
- ~15 payments including refunds

The seeder is **idempotent** — it checks `if (await context.Instructors.AnyAsync())` and skips if data exists. Safe to run on every startup.

---

## Deployment Evidence

> All evidence stored in the project's shared drive folder. Screenshots referenced below.

### 1. Database Name
**[Database name](https://drive.google.com/file/d/1EW9iWk_E1DAV39LOiyp_o6Q3vqbtbDP5/view?usp=drive_link)
*Azure Portal → SQL Database → Overview showing `TechMasterTrainingCenterDb`.*

### 2. Connection Settings (Password Hidden)
**[Connection settings](docs/evidence/02-connection-settings.png)
*App Service → Configuration → Connection Strings. The password field is masked by Azure.*

### 3. Remote Tables
**[Remote tables](https://drive.google.com/file/d/1Yk53uy3KDJkWC5xyZLzUDglEvi5fxQsj/view?usp=drive_link)
*Azure SQL Query Editor showing `Students`, `Instructors`, `TrainingTracks`, `Enrollments`, `Payments`, `__EFMigrationsHistory`.*

### 4. Migration History
**[Migration history](https://drive.google.com/file/d/1yo18dGcFiXckg4ksXjaxm16QPCaGRqlM/view?usp=drive_link)
*`SELECT * FROM __EFMigrationsHistory` — confirms `InitialCreate` was applied.*

### 5. Seed Data
**[Seed data](https://drive.google.com/file/d/1LjtAs-_MMEY7EDweX_q8h0XX-NlJd2eU/view?usp=drive_link)
*`SELECT COUNT(*) FROM Instructors` (and other tables) showing seeded rows.*

### 6. Live Swagger UI
**[Swagger](https://techmasterapi.azurewebsites.net/swagger/index.html)
 — all controllers listed.*

### 7. GET Endpoint Online
**[GET online](https://drive.google.com/file/d/1QHbmCXa0NhyH8Tnn_vctONl3R-3iNogC/view?usp=drive_link)
*`GET /api/instructors` via Swagger — returns 200 with seeded instructors.*

### 8. POST Endpoint Online
**[POST online](https://drive.google.com/file/d/11mYf5dT54QPx0-Gpz175Rjr-hA7Dh-hq/view?usp=drive_link)
*`POST /api/students` via Swagger — returns 201 Created.*

### 9. Postman Request
**[Postman](https://drive.google.com/drive/folders/1aQ_JFfQk6Si3uYOcdAJ-lPmFR57IYpni?usp=drive_link)
*Postman hitting the live URL with a real request/response.*

### 10. Drive Deployment Evidence
**[Drive](https://drive.google.com/drive/folders/1wGYd3fzJef-AGyS9imZ0jFJq8tqIRMFk?usp=drive_link)
*Shared drive folder containing all screenshots and the demo video.*

---

## Demo Video

📹 **[Watch the demo](https://drive.google.com/file/d/1448XpWoWpC60w1xgf5XKmt4WUmCWDTx-/view?usp=drive_link)** (stored in the shared drive)

The video covers:
- Opening the live Swagger URL in a browser
- Calling a GET endpoint (returns seeded data)
- Calling a POST endpoint (creates a record)
- Azure Portal tour (App Service config, SQL Database overview)
- **No passwords or connection strings are visible at any point**

---

## Known Limitations

- **Free tier runtime limit**: The App Service is on Free (F1), which has a 60-minute daily runtime cap. If the API returns a "site disabled" page, it has hit the cap; it resets the next day. Upgrade to Basic (B1) for 24/7 availability.
- **No authentication yet**: All endpoints are public. Production rollout should add JWT or Azure AD B2C.
- **No automated tests**: Test coverage is planned.
- **Seeder uses test credentials**: All seeded emails use `@*.test` domains — safe placeholders.

---

## Author

**Abdulrahaman Mohamed**
Backend .NET Developer — TechMaster Academy Training Program

- GitHub: [Abd-ErrahmanAbuAl-Hassan](https://github.com/Abd-ErrahmanAbuAl-Hassan/)
- LinkedIn: [abdulrahman-abu-al-hassan](https://www.linkedin.com/in/abdulrahman-abu-al-hassan)

---
