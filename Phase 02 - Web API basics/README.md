# Phase 02 - ASP.NET Core Web API Basics

Phase 02 is the transition from console-based applications to real HTTP
APIs. It focuses on building clear, testable and review-ready ASP.NET Core
Web APIs before introducing databases and Entity Framework Core in Phase 03.

## Student

- **Name:** Abdulrahman Mohamed
- **Track:** ASP.NET Backend Career Training
- **Academy:** TechMaster Academy

## Phase Summary

This phase contains practical exercises and small API projects covering:

- **API fundamentals:** HTTP requests and responses, controllers, routing,
  query strings, route parameters, request bodies and status codes.
- **REST and routing drills:** 15 focused exercises for health checks,
  calculations, conversions, notes, search, pagination, headers and standard
  error responses.
- **Student Management API:** An in-memory CRUD API with DTOs, services,
  validation, search, filtering, pagination and statistics.
- **Products & Categories API:** Related resources with category validation,
  product filtering, inventory visibility and stock reports.
- **Book Store API:** A larger in-memory project combining books, authors,
  categories, relationships, validation, pagination and summary reports.
- **API delivery standards:** Swagger/OpenAPI documentation, Postman testing,
  evidence collection and refactoring poorly structured API code.
- **Interview preparation:** Short explanations of REST, controllers, DTOs,
  services, dependency injection, validation, status codes and API debugging.

## Phase Deliverables

By the end of the phase, the repository should contain:

1. A working API setup project.
2. 15 REST and routing drills.
3. Student Management, Products & Categories, and Book Store APIs.
4. Swagger documentation and a Postman collection with success and failure
	cases.
5. README documentation, screenshots and interview answers.
6. A refactored API example demonstrating clean controller, DTO and service
	separation.

## Repository Structure

```text
Phase 02 - Web API basics/
├── README.md
├── Task 00 - API Workspace Setup/
├── Task 01 - REST & Routing Drills/
├── Task 02 - Student Management API/
├── Task 03 - Products & Categories API/
├── Task 04 - Book Store API/
├── Task 05 - Swagger & Postman Evidence/
├── Task 06 - API Standards & Refactor Pack/
└── Task 07 - Interview Answers/
```

## Current Setup Project

The initial project is located in `Task 00 - API Workspace Setup` and targets
.NET 9. It includes:

- Controller-based API configuration.
- Swagger/OpenAPI documentation.
- HTTPS redirection.
- A starter `WeatherForecast` endpoint for verifying the workspace.

## Prerequisites

- .NET 9 SDK or a compatible newer SDK.
- Visual Studio or Visual Studio Code.
- Git and GitHub.
- Postman for request testing.

## How To Run

From the phase directory, run:

```powershell
dotnet run --project ".\Task 00 - API Workspace Setup\Task 00 - API Workspace Setup.csproj"
```

When the application starts, open Swagger:

- `https://localhost:7241/swagger`
- `http://localhost:5036/swagger`

The HTTPS development certificate may prompt a browser warning on a local
machine. This is expected for local development.

## API Standards

Each endpoint should have a clear route, request shape, response shape,
validation rule and status code. Controllers should coordinate HTTP concerns,
DTOs should define public request and response contracts, and services should
own business logic and in-memory data operations.

Expected status codes include:

- `200 OK` for successful reads and updates.
- `201 Created` when a resource is created.
- `204 No Content` for a successful delete without a response body.
- `400 Bad Request` for invalid input.
- `404 Not Found` when the requested resource does not exist.

## Evidence Checklist

- Swagger opens and lists the implemented endpoints.
- Postman requests cover successful and invalid scenarios.
- At least three error responses are documented.
- README files explain how each task runs and is tested.
- Screenshots and exported Postman collections are stored in the relevant
  task folder.

## Learning Outcome

This phase builds the foundation for database-backed APIs. The projects use
in-memory collections intentionally, allowing the focus to remain on HTTP,
API design, validation and code organization. In Phase 03, the service and
model structure can be adapted to persistent storage with Entity Framework
Core.