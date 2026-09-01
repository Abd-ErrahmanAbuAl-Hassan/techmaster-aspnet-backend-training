# 🧩 REST Routing Drill Pack

A collection of small, focused **ASP.NET Core Web API** endpoints designed as practice drills for core REST API concepts — routing, route/query parameters, request bodies, status codes, headers, and basic CRUD with search and pagination.

---

## 📖 Table of Contents

- [Features](#-features)
- [Project Structure](#-project-structure)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Sample Requests](#-sample-requests)
- [Screenshots](#-screenshots)
- [Notes](#-notes)

---

## ✨ Features

- **Health check** endpoint for service status
- **Routing drills**: route parameters (`echo/{name}`), query parameters (`calculator/add`)
- **Utility endpoints**: Celsius → Fahrenheit conversion, grade calculator
- **Notes CRUD** — create, read, update, delete, with search and pagination
- **Header handling** — reading a custom request header (`X-Student-Name`)
- **HTTP status code drills** — dedicated endpoints demonstrating `200`, `201`, `204`, `400`, and `404` responses
- **Structured error response drills** — a switch-based demo endpoint returning different error shapes

---

## 🗂 Project Structure

```
task-01-rest-routing-drill-pack/
├── README.md
├── Task_01_REST_Routing_Drill_Pack/
│   ├── Controllers/
│   │   └── DrillsController.cs
│   ├── Entities/
│   │   └── Note.cs
│   ├── DTOs/
│   │   ├── CreateNoteRequest.cs
│   │   └── UpdateNoteRequest.cs
│   ├── Services/
│   │   └── ConverterService.cs
│   └── Program.cs
```

---

## 🛠 Tech Stack

- **.NET / ASP.NET Core Web API**
- **C#**
- In-memory data store (static `List<Note>` — no database required)
- Swagger / OpenAPI for interactive API testing

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (7.0 or later recommended)
- An IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider

### Installation & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/<your-username>/task-01-rest-routing-drill-pack.git
   cd task-01-rest-routing-drill-pack/Task_01_REST_Routing_Drill_Pack
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open the API documentation (Swagger UI)**
   ```
   https://localhost:<port>/swagger
   ```

---

## 📡 API Endpoints

All routes are prefixed with `/api/drills`.

### General / Routing Drills

| Method | Endpoint                              | Description                                           |
|--------|------------------------------------------|----------------------------------------------------------|
| GET    | `/health`                                | Service health check (status, service name, UTC time)    |
| GET    | `/tools/echo/{name}`                     | Echoes back a greeting using a route parameter            |
| GET    | `/calculator/add?a={a}&b={b}`            | Adds two numbers (query parameters); handles negative `b` as subtraction in the message |
| GET    | `/converter/celsius-to-fahrenheit?celsius={c}` | Converts a Celsius value to Fahrenheit               |
| GET    | `/grades/calculate?score={score}`        | Returns a letter grade (A–F) for a score between 0–100     |

### Notes CRUD

| Method | Endpoint                 | Description                                    |
|--------|---------------------------|--------------------------------------------------|
| POST   | `/notes`                  | Create a new note                                 |
| GET    | `/notes`                  | Get all notes                                      |
| GET    | `/notes/pagination`       | Get notes with pagination (`page`, `pageSize`)     |
| GET    | `/notes/{id}`             | Get a note by ID                                    |
| PUT    | `/notes/{id}`             | Update a note by ID                                  |
| DELETE | `/notes/{id}`             | Delete a note by ID                                   |
| GET    | `/notes/search?searchTerm={term}` | Search notes by title or content              |

### Headers & Status Code Drills

| Method | Endpoint                          | Description                                                |
|--------|--------------------------------------|----------------------------------------------------------------|
| GET    | `/request-info`                      | Reads the `X-Student-Name` request header and echoes request info |
| GET    | `/status-codes/success`              | Returns `200 OK`                                                 |
| POST   | `/status-codes/created`              | Returns `201 Created` with a `Location` header                    |
| DELETE | `/status-codes/no-content`           | Returns `204 No Content`                                           |
| GET    | `/status-codes/bad-request`          | Returns `400 Bad Request`                                          |
| GET    | `/status-codes/not-found`            | Returns `404 Not Found`                                             |
| GET    | `/errors/demo?errorType={type}`      | Returns a structured error body; `type` can be `bad-request`, `not-found`, or anything else (defaults to a `500` demo) |

---

## 🔍 Sample Requests

**Echo drill**
```http
GET /api/drills/tools/echo/Ahmed
```

**Add two numbers**
```http
GET /api/drills/calculator/add?a=10&b=5
```

**Celsius to Fahrenheit**
```http
GET /api/drills/converter/celsius-to-fahrenheit?celsius=25
```

**Create a note**
```http
POST /api/drills/notes
Content-Type: application/json

{
  "title": "Study REST APIs",
  "content": "Review routing, status codes, and headers."
}
```

**Search notes**
```http
GET /api/drills/notes/search?searchTerm=rest
```

**Read a custom header**
```http
GET /api/drills/request-info
X-Student-Name: Ahmed Mohamed
```

**Error demo**
```http
GET /api/drills/errors/demo?errorType=not-found
```

---

## 🖼 Screenshots

Below are screenshots demonstrating the API in action (Swagger UI / Postman testing).

> 📌 **[View Screenshots on Google Drive - Postman](https://drive.google.com/drive/folders/19Iz2zmBnoHvWhcA32ew9xWl3HLlFVLDg?usp=drive_link)**
> 📌 **[View Screenshots on Google Drive - Swagger](https://drive.google.com/drive/folders/1uvGgOxgvZ2-sGbZMlNU-__wAUw-qso4z?usp=drive_link)**

| Description                          | Preview |
|----------------------------------------|---------|
| Swagger UI — All Endpoints             | *(https://drive.google.com/file/d/1vroiRIG3kVYowFpWRSUKMqIxpG064rz_/view?usp=drive_link)* |
| GET /api/drills/health — Response      | *(https://drive.google.com/file/d/11J5DA45RjdlyhLdtu-a0cU9Nt8u_40fL/view?usp=drive_link)* |
| Notes CRUD — Create & Search           | *(https://drive.google.com/file/d/1M0MTJ2N9w2Ga78YISryqOCJMLtLIdsnr/view?usp=drive_link)* |
| Status Code Drills — 400/404 Responses | *(https://drive.google.com/file/d/11J5DA45RjdlyhLdtu-a0cU9Nt8u_40fL/view?usp=drive_link)* |

---

## 📝 Notes

- This project is a **learning drill pack**, not a production service — each endpoint targets a specific REST/API concept rather than forming a single cohesive domain.
- Notes are stored **in-memory** (a static `List<Note>`), so all data resets whenever the application restarts.
- Response bodies use anonymous objects rather than a shared `Result<T>` wrapper, since the focus here is on exercising routing, status codes, and payload shapes directly.

---

### 👤 Author

Project developed as part of a mini-project task focused on practicing core ASP.NET Core Web API routing and REST fundamentals.