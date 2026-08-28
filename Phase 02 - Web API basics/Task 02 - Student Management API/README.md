
# Student Management API

## 📋 Project Summary

**Task 02 - Student Management API** is an ASP.NET Core Web API built with .NET 9 that provides complete CRUD operations for managing student records. The API handles student creation, retrieval, updating, status management, and deletion with comprehensive validation and error handling.

### Key Features
- ✅ Create new student records with validation
- ✅ Retrieve all students with pagination support
- ✅ Get individual student details by ID
- ✅ Update student information
- ✅ Update student status (Active/Inactive)
- ✅ Delete student records
- ✅ Retrieve student statistics
- ✅ Filter and paginate student lists
- ✅ Comprehensive error handling with meaningful responses
- ✅ Swagger/OpenAPI documentation

---

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK or later
- Visual Studio 2022 (recommended)
- Postman or any REST client (optional, for manual testing)

### Installation
1. Clone the repository:
   git clone https://github.com/Abd-ErrahmanAbuAl-Hassan/techmaster-aspnet-backend-training.git
   cd Task_02___Student_Management_API

2. Restore NuGet packages:
   dotnet restore

3. Run the application:
   dotnet run

4. The API willbe available at:
- **HTTP**: `https://localhost:5001`
   - **Swagger UI**: `https://localhost:5001/swagger`

---

## 📡 API Routes & Endpoints

### Base URL
https://localhost:5001/api/students


### 1. Create Student
**POST** `/api/students/create`

Creates a new student record.

**Request Body:**
{
  "fName": "Ahmed",
  "lName": "Hassan",
  "email": "ahmed@example.com",
  "phoneNumber": "01012345678",
  "trackName": "Backend Development",
  "linkedInURL": "https://linkedin.com/in/ahmed",
  "githubURL": "https://github.com/ahmed"
}

**Response (201 Created):**
{
  "success": true,
  "message": "Student created successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "fName": "Ahmed",
    "lName": "Hassan",
    "fullName": "Ahmed Hassan",
    "email": "ahmed@example.com",
    "phoneNumber": "01012345678",
    "trackName": "Backend Development",
    "linkedInURL": "https://linkedin.com/in/ahmed",
    "githubURL": "https://github.com/ahmed",
    "enrollmentDate": "2026-08-28T10:30:00",
    "isActive": true
  },
  "errors": null,
  "errorCode": 0
}

**Validation Rules:**
- First Name: Required, non-empty
- Last Name: Required, non-empty
- Email: Required, valid email format
- Phone Number: Required, Egyptian phone format (01X + 8 digits)
- Track Name: Required, non-empty

---

### 2. Get All Students
**GET** `/api/students/all`

Retrieves all students with optional pagination and filtering.

**Query Parameters:**
- `page` (optional): Page number (starts at 1)
- `pageSize` (optional): Number of records per page (max 50)

**Example Requests:**
/api/students/all
/api/students/all?page=1&pageSize=10

**Response (200 OK):**
{
  "success": true,
  "message": "Students retrieved successfully",
  "data": [
    {
      "fullName": "Ahmed Hassan",
      "email": "ahmed@example.com",
      "phoneNumber": "01012345678",
      "trackName": "Backend Development",
      "linkedInURL": "https://linkedin.com/in/ahmed",
      "githubURL": "https://github.com/ahmed",
      "enrollmentDate": "2026-08-28T10:30:00",
      "isActive": true
    }
  ],
  "errors": null,
  "errorCode": 0
}

---

### 3. Get Student by ID
**GET** `/api/students/{id}`

Retrieves a specific student by their ID.

**Path Parameter:**
- `id` (required): Student GUID

**Example Request:**
/api/students/550e8400-e29b-41d4-a716-446655440000


**Response (200 OK):**
{
  "success": true,
  "message": "Student retrieved successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "fName": "Ahmed",
    "lName": "Hassan",
    "fullName": "Ahmed Hassan",
    "email": "ahmed@example.com",
    "phoneNumber": "01012345678",
    "trackName": "Backend Development",
    "enrollmentDate": "2026-08-28T10:30:00",
    "isActive": true
  },
  "errors": null,
  "errorCode": 0
}

---

### 4. Get Student Statistics
**GET** `/api/students/stats`

Retrieves statistics about students in the system.

**Response (200 OK):**
{
  "success": true,
  "message": "Statistics retrieved successfully",
  "data": {
    "totalStudents": 5,
    "activeStudents": 4,
    "inactiveStudents": 1,
    "byTrack": {
      "Backend Development": 3,
      "Frontend Development": 2
    }
  },
  "errors": null,
  "errorCode": 0
}

---

### 5. Update Student
**PUT** `/api/students/{id}`

Updates an existing student's information.

**Path Parameter:**
- `id` (required): Student GUID

**Request Body:**
{
  "fName": "Ahmed",
  "lName": "Hassan",
  "email": "newemail@example.com",
  "phoneNumber": "01098765432",
  "trackName": "Full Stack Development",
  "linkedInURL": "https://linkedin.com/in/ahmed-new",
  "githubURL": "https://github.com/ahmed-new"
}

**Response (200 OK):**
{
  "success": true,
  "message": "Student updated successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "fName": "Ahmed",
    "lName": "Hassan",
    "email": "newemail@example.com",
    "trackName": "Full Stack Development"
  },
  "errors": null,
  "errorCode": 0
}

---

### 6. Update Student Status
**PATCH** `/api/students/{id}/status`

Updates only the student's active/inactive status.

**Path Parameter:**
- `id` (required): Student GUID

**Request Body:**
{
  "isActive": false
}

**Response (200 OK):**
{
  "success": true,
  "message": "Student status updated successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "isActive": false
  },
  "errors": null,
  "errorCode": 0
}

---

### 7. Delete Student
**DELETE** `/api/students/{id}/delete`

Deletes a student record permanently.

**Path Parameter:**
- `id` (required): Student GUID

**Example Request:**
/api/students/550e8400-e29b-41d4-a716-446655440000/delete

**Response (200 OK):**
{
  "success": true,
  "message": "Student deleted successfully",
  "data": null,
  "errors": null,
  "errorCode": 0
}

---

## 🧪 Testing Guide

### Option 1: Using Swagger UI (Recommended)
1. Start the application
2. Navigate to `https://localhost:5001/swagger`
3. Expand any endpoint
4. Click "Try it out"
5. Fill in the required parameters
6. Click "Execute"

### Option 2: Using Postman
1. Import the API routes listed above
2. Set the base URL to `https://localhost:5001`
3. For each endpoint, set the method (GET, POST, PUT, PATCH, DELETE)
4. Add request bodies where applicable
5. Send requests and verify responses

### Option 3: Using cURL
# Create a student
curl -X POST https://localhost:5001/api/students/create \
  -H "Content-Type: application/json" \
  -d '{
    "fName": "Ahmed",
    "lName": "Hassan",
    "email": "ahmed@example.com",
    "phoneNumber": "01012345678",
    "trackName": "Backend Development"
  }'

# Get all students
curl -X GET https://localhost:5001/api/students/all

# Get student by ID
curl -X GET https://localhost:5001/api/students/{id}

# Update student
curl -X PUT https://localhost:5001/api/students/{id} \
  -H "Content-Type: application/json" \
  -d '{"fName":"Updated","lName":"Name"}'

# Delete student
curl -X DELETE https://localhost:5001/api/students/{id}/delete

---

## 📸 Swagger Screenshots & Evidence

All Swagger/API testing screenshots and evidence have been documented and stored in Google Drive:

**📁 Google Drive Folder:** [Student Management API - Swagger Evidence](https://drive.google.com/drive/folders/YOUR_FOLDER_ID_HERE)

### Screenshots Included:
- ✅ Swagger UI Dashboard
- ✅ Create Student Endpoint Test
- ✅ Get All Students Endpoint Test
- ✅ Get Student by ID Test
- ✅ Update Student Test
- ✅ Update Student Status Test
- ✅ Delete Student Test
- ✅ Get Statistics Test
- ✅ Pagination Testing
- ✅ Validation Error Examples

*Note: Replace `YOUR_FOLDER_ID_HERE` with the actual Google Drive folder link containing the screenshots.*

---

## ⚙️ Configuration

### appsettings.json
The application uses the default ASP.NET Core configuration. Modify as needed for your environment.

### Swagger Configuration
Swagger is enabled in Development environment. Access it at `/swagger` endpoint.

---

## 📝 Error Responses

The API returns standardized error responses:

**400 Bad Request:**
{
  "success": false,
  "message": "Validation errors.",
  "errors": [
    "First name is required.",
    "Invalid email address."
  ],
  "errorCode": 400
}

**404 Not Found:**
{
  "success": false,
  "message": "Student not found.",
  "errors": [],
  "errorCode": 404
}

**409 Conflict:**
{
  "success": false,
  "message": "Cannot perform this operation.",
  "errors": ["Student status conflict"],
  "errorCode": 409
}

**500 Internal Server Error:**
{
  "success": false,
  "message": "An error occurred while processing your request.",
  "errors": ["Server error details"],
  "errorCode": 500
}

---

## 🏗️ Project Structure

Task_02___Student_Management_API/
├── Controllers/
│   └── StudentsController.cs
├── Services/
│   └── StudentService.cs
├── DTOs/
│   ├── CreateStudentRequest.cs
│   ├── UpdateStudentRequest.cs
│   ├── UpdateStudentStatusRequest.cs
│   └── StudentResponse.cs
├── Entities/
│   └── Student.cs
├── Utilities/
│   ├── Result.cs
│   └── Filter.cs
├── Program.cs
└── README.md

---

## 🔧 Technology Stack

- **Framework**: ASP.NET Core 9
- **Language**: C# 13.0
- **Architecture**: RESTful API
- **Documentation**: Swagger/OpenAPI
- **Dependency Injection**: Built-in .NET Core DI

---

## 📌 Important Notes

1. **Data Persistence**: The current implementation uses in-memory storage. Data will be lost when the application restarts.
2. **Phone Validation**: Accepts Egyptian phone numbers in format `01X + 8 digits` (e.g., `01012345678`)
3. **Email Validation**: Standard email format validation is applied
4. **Singleton Service**: `StudentService` is registered as a singleton to maintain data during the session

---

## ✨ Author

Created as Task 02 for TechMaster Backend Training Program

**Repository**: [techmaster-aspnet-backend-training](https://github.com/Abd-ErrahmanAbuAl-Hassan/techmaster-aspnet-backend-training)

This revised README maintains the original structure while enhancing clarity and coherence, ensuring that all necessary information is presented in a logical flow.