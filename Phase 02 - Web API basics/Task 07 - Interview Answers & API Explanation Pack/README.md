# Task 07 - Web API Interview Answers & Explanation Pack

## 📖 Overview
This repository contains the answers and explanations for the Web API phase of Task 07. The goal is to demonstrate the ability to explain technical concepts clearly, a crucial skill for professional software development.

---

## 🎯 Required Questions & Answers

### 1. What does REST mean in the context of Web APIs?
**Answer:**
REST => **Representational State Transfer**. It is an architectural style for designing APIs. It relies on stateless, client-server communication, where standard HTTP methods are used to manipulate resources identified by URLs.

**Example:**
Instead of `GetStudentById?id=1`, a RESTful API uses `GET api/students/{id}`.

### 2. What is the difference between GET, POST, PUT, PATCH and DELETE?
**Answer:**
These are HTTP methods (verbs) that define the action to be performed on a resource:
*   **GET:** Retrieve data.
*   **POST:** Create a new resource.
*   **PUT:** Update an entire existing resource (replace).
*   **PATCH:** Update part of an existing resource (modify).
*   **DELETE:** Remove a resource.

**Example:**
`GET /api/studnets` (list all), `POST /api/students` (add new).

### 3. When should an API return 200, 201, 204, 400 and 404?
**Answer:**
*   **200 OK:** Standard success.
*   **201 Created:** A new resource was successfully created.
*   **204 No Content:** Success, but no data to return.
*   **400 Bad Request:** Client sent invalid data.
*   **404 Not Found:** The requested resource does not exist.

**Example:**
`DELETE /api/students/{id}` returns 204; `GET /api/students/999` returns 404.

### 4. What is the difference between route parameters and query parameters?
**Answer:**
*   **Route parameters** are part of the URL path and usually identify a specific resource.
*   **Query parameters** are appended to the URL after a `?` and are used for filtering, sorting, or pagination.

### 5. What is the role of a controller in ASP.NET Core Web API?
**Answer:**
The controller handles incoming HTTP requests. It is responsible for:
1.  Receiving the request.
2.  Validating input.
3.  Calling the appropriate service/business logic.
4.  Returning an HTTP response (status code + data).

**Example:**
`ProductsController` receives `GET /products?...`, calls `_productService.GetAllProducts(filter)`, and returns `Ok(result)`.

### 6. Why should we use DTOs instead of exposing models directly?
**Answer:**
DTOs (Data Transfer Objects) separate the internal database models from the API contract. This prevents over-posting, hides sensitive data (like passwords), and allows the API shape to change without breaking the database schema.

### 7. Why should business logic not stay inside the controller?
**Answer:**
Controllers should only handle HTTP concerns (routing, status codes). Business logic belongs in a service layer. This makes the code testable, reusable, and easier to maintain (Single Responsibility Principle).

### 8. What is Dependency Injection and why is it useful?
**Answer:**
Dependency Injection (DI) is a design pattern where a class receives its dependencies from an external source rather than creating them itself. It promotes loose coupling, makes testing easier (via mocking), and manages object lifetimes.

```csharp
//program.cs
builder.Services.AddSingleton<ProductService>();
```

```csharp
//product controller
private readonly ProductService _productService;

public ProductsController(ProductService productService)
{
    _productService = productService;
}
```

### 9. Where can validation happen in a Web API?
**Answer:**
Validation can happen at multiple levels:
1.  **Model Binding/Data Annotations:** In the DTO.
2.  **Controller:** Checking `ModelState.IsValid`.
3.  **Service Layer:** Business rule validation.

### 10. What is Swagger/OpenAPI used for?
**Answer:**
Swagger is used to document API endpoints. It provides a UI where developers can see available endpoints, required parameters, and test the API directly from the browser.

### 11. Why do we need Postman if Swagger already exists?
**Answer:**
Swagger is great for documentation and simple testing. Postman is a more powerful API client. It is better for:
*   Saving complex request collections.
*   Writing automated test scripts.
*   Handling environment variables.
*   Testing authentication flows.

### 12. What makes an API response professional and predictable?
**Answer:**
*   **Consistent structure:** Same JSON shape for success and error responses.
*   **Correct status codes:** Using 200, 201, 400, 404 appropriately.
*   **Clear error messages:** Helpful details when something fails.
*   **No sensitive data leakage:** Never returning stack traces or passwords.

### 13. How would you implement search and filtering in an API?
**Answer:**
Use query parameters to accept filter criteria and apply them using LINQ in the service layer.

**Example:**
```csharp
public class PFilter
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public int? LowStockThreshold { get; set; }
    public bool? IsAvilable { get; set; }
}
```
```csharp
[HttpGet]
public IActionResult GetAll([FromQuery] PFilter filter)
{
    var result = _productService.GetAllProducts(filter);

    if (!result.Success && result.ErrorCode == 400)
        return BadRequest(result);

    if (!result.Success && result.ErrorCode == 404)
        return NotFound(result);

    return Ok(result);
}
```
```csharp
public Result<List<ProductResponse>> GetAllProducts(PFilter filter)
{
    if (!_products.Any())
        return new Result<List<ProductResponse>>
        {
            Success = false,
            Message = "No Products Exists, create the first one.",
            ErrorCode = 404
        };

    var errors = new List<string>();
    var products = _products.Select(p => MapToProductResponse(p)).ToList();


    if (filter.SearchTerm is not null)
    {
        products = products.Where(s => s.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)
                                    || s.SupplierName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    if (filter.IsAvilable is not null)
    {
        products = products.Where(s => s.IsAvailable == filter.IsAvilable).ToList();
    }

    if (filter.MaxPrice != null && filter.MinPrice != null)
    {
        if (filter.MinPrice > filter.MaxPrice) errors.Add("Minimum price can not be lower than maximum price.");
        else
        {
            products = products.Where(p => p.Price >= filter.MinPrice && p.Price <= filter.MaxPrice).ToList();
        }

    }

    if (filter.Category is not null)
    {
        products = products.Where(p => p.Category == filter.Category).ToList();
    }
    if (filter.LowStockThreshold is not null)
    {
        products = products.Where(p => p.StockQuantity > 0 && p.StockQuantity <= filter.LowStockThreshold).ToList();
    }
    var count = products.Count;
    if (filter.PageSize is not null && filter.Page is not null)
    {
        if (filter.Page < 1) errors.Add("Page number must be greater than 1.");
        if (filter.PageSize < 1) errors.Add("Page size must be greater than 1.");
        if (filter.PageSize > 50) errors.Add("Page size must be less than 50.");
        if (errors.Any()) return new Result<List<ProductResponse>>
        {
            Success = false,
            Message = "Pagination parameters are not valid.",
            Errors = errors,
            ErrorCode = 400
        };

        products = products.Skip((int)((filter.Page - 1) * filter.PageSize))
                           .Take((int)filter.PageSize).ToList();

    }

    if (!products.Any())
        return new Result<List<ProductResponse>>
        {
            Success = false,
            Message = "No Categories found.",
            ErrorCode = 404
        };

    return new Result<List<ProductResponse>>
    {
        Success = true,
        Message = $"Successfully retrieve ({count}) products.",
        Data = products
    };
}
```
`GET api/products?category=electronics`
In code: `products = products.Where(p => p.Category == filter.Category).ToList();`.

### 14. Why is pagination important in APIs?
**Answer:**
Pagination prevents the API from returning massive amounts of data at once, which could crash the server or slow down the client. It improves performance and user experience.

**Example:**
`GET /Categories?page=1&pageSize=20` returns only 20 items instead of all 10,000.

### 15. What is the difference between UI, controller, service and data storage?
**Answer:**
*   **UI (Client):** Displays data to the user.
*   **Controller:** Handles HTTP requests/responses (API layer).
*   **Service:** Contains business logic and rules.
*   **Data Storage:** The database where data is persisted.

**Example:**
UI sends request → Controller receives → Service processes → Data Storage saves → Response returns.

### 16. What should reviewers see in your commit history?
**Answer:**
A clean, logical history showing incremental progress. Each commit should have a clear message describing what was done. Reviewers should see that features were built step-by-step, not one giant "final commit."

### 17. How do you prove that your API works without running it on the reviewer device?
**Answer:**
By providing:
*   **Unit/Integration Tests:** Automated tests that verify behavior.
*   **Swagger/Postman Documentation:** Screenshots or exported collections.
*   **README:** Clear instructions on how to run and test the API.

### 18. How do you investigate an endpoint that returns 500?
**Answer:**
A 500 Internal Server Error means an unhandled exception occurred. To investigate:
1.  Check the server logs (console or file logging).
2.  Look at the stack trace to find the exact line of code.
3.  Reproduce the issue locally.
4.  Add try-catch blocks or global exception handling middleware.

### 19. Why should we not trust request data from users?
**Answer:**
User input can be malicious (SQL injection, XSS) or simply incorrect. Always validate, sanitize, and never trust data coming from the client. Use DTOs with data annotations and parameterized queries.

### 20. Why is in-memory storage not enough for real applications?
**Answer:**
In-memory storage (like a static list) is lost when the application restarts. It is not scalable across multiple servers and cannot handle complex queries. Real applications need a persistent database (SQL, NoSQL).
