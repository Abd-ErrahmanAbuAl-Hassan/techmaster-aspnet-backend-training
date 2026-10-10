using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TrainingCenter.API.Middleware
{
    /// <summary>
    /// Global exception handling middleware for capturing and formatting all unhandled exceptions.
    /// Converts exceptions into standardized Result pattern responses with appropriate status codes.
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred for {Method} {Path}. Exception: {@Exception}",
                context.Request.Method, context.Request.Path, exception);

            context.Response.ContentType = "application/json";

            var (statusCode, message, errors) = MapExceptionToResponse(exception);

            context.Response.StatusCode = statusCode;

            var response = new
            {
                success = false,
                message = message,
                statusCode = statusCode,
                errors = errors,
                timestamp = DateTime.UtcNow
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(json);
        }

        /// <summary>
        /// Maps exception types to appropriate HTTP status codes, messages, and error details.
        /// </summary>
        private (int StatusCode, string Message, List<string> Errors) MapExceptionToResponse(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException ex => (
                    StatusCodes.Status400BadRequest,
                    "Invalid request: A required argument was null.",
                    new List<string> { ex.ParamName ?? "Unknown parameter" }
                ),

                ArgumentException ex => (
                    StatusCodes.Status400BadRequest,
                    "Invalid request: An argument validation failed.",
                    new List<string> { ex.Message }
                ),

                InvalidOperationException ex => (
                    StatusCodes.Status400BadRequest,
                    "The requested operation cannot be completed in the current state.",
                    new List<string> { ex.Message }
                ),

                KeyNotFoundException ex => (
                    StatusCodes.Status404NotFound,
                    "The requested resource was not found.",
                    new List<string> { ex.Message }
                ),

                UnauthorizedAccessException ex => (
                    StatusCodes.Status403Forbidden,
                    "Access denied: You do not have permission to perform this action.",
                    new List<string> { ex.Message }
                ),

                TimeoutException ex => (
                    StatusCodes.Status408RequestTimeout,
                    "The request took too long to complete. Please try again.",
                    new List<string> { ex.Message }
                ),

                DbUpdateConcurrencyException ex => (
                    StatusCodes.Status409Conflict,
                    "Concurrency conflict: The record was modified by another user. Please refresh and try again.",
                    new List<string> { ex.Message }
                ),

                DbUpdateException ex => (
                    StatusCodes.Status409Conflict,
                    "A database error occurred. The operation could not be completed.",
                    new List<string> { ex.InnerException?.Message ?? ex.Message }
                ),


                _ => (
                    StatusCodes.Status500InternalServerError,
                    "An unexpected server error occurred. Please contact support if the problem persists.",
                    new List<string> { exception.Message }
                )
            };
        }
    }
}