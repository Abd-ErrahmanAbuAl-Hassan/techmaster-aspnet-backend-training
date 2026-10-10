namespace TrainingCenter.Domain.Results
{
    public class Result<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public T Data { get; }
        public List<string> Errors { get; }
        public int StatusCode { get; }

        private Result(bool success, string message, T data, List<string> errors, int statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
            StatusCode = statusCode;
        }


        public static Result<T> SuccessResult(T data, string message = "Operation completed successfully.", int statusCode = 200)
        {
            return new Result<T>(
                success: true,
                message: message,
                data: data,
                errors: new List<string>(),
                statusCode: statusCode
            );
        }

        public static Result<T> FailureResult(string message, List<string> errors, int statusCode = 500)
        {
            return new Result<T>(
                success: false,
                message: message,
                data: default!,
                errors: errors ?? new List<string>(),
                statusCode: statusCode
            );
        }

        public static Result<T> FailureResult(string message, string error, int statusCode = 500)
        {
            return new Result<T>(
                success: false,
                message: message,
                data: default!,
                errors: new List<string> { error },
                statusCode: statusCode
            );
        }

        public static Result<T> NotFoundResult(string message)
        {
            return new Result<T>(
                success: false,
                message: message,
                data: default!,
                errors: new List<string>(),
                statusCode: 404
            );
        }

        public static Result<T> ConflictResult(string message, string error = null)
        {
            return new Result<T>(
                success: false,
                message: message,
                data: default!,
                errors: string.IsNullOrEmpty(error) ? new List<string>() : new List<string> { error },
                statusCode: 409
            );
        }

        public static Result<T> UnauthorizedResult(string message)
        {
            return new Result<T>(
                success: false,
                message: message,
                data: default!,
                errors: new List<string>(),
                statusCode: 401
            );
        }
        public static Result<T> ForbiddenResult(string message)
        {
            return new Result<T>(
                success: false,
                message: message,
                data: default!,
                errors: new List<string>(),
                statusCode: 403
            );
        }

        public static Result<T> ValidationErrorResult(string message, List<string> errors)
        {
            return new Result<T>(
                success: false,
                message: message,
                data: default!,
                errors: errors ?? new List<string>(),
                statusCode: 400
            );
        }
    }

    public class Result
    {
        public bool Success { get; }
        public string Message { get; }
        public List<string> Errors { get; }
        public int StatusCode { get; }

        private Result(bool success, string message, List<string> errors, int statusCode)
        {
            Success = success;
            Message = message;
            Errors = errors;
            StatusCode = statusCode;
        }

        public static Result SuccessResult(string message = "Operation completed successfully.", int statusCode = 200)
        {
            return new Result(
                success: true,
                message: message,
                errors: new List<string>(),
                statusCode: statusCode
            );
        }

        public static Result FailureResult(string message, List<string> errors, int statusCode = 500)
        {
            return new Result(
                success: false,
                message: message,
                errors: errors ?? new List<string>(),
                statusCode: statusCode
            );
        }

        public static Result FailureResult(string message, string error, int statusCode = 500)
        {
            return new Result(
                success: false,
                message: message,
                errors: new List<string> { error },
                statusCode: statusCode
            );
        }

        public static Result NotFoundResult(string message)
        {
            return new Result(
                success: false,
                message: message,
                errors: new List<string>(),
                statusCode: 404
            );
        }

        public static Result ConflictResult(string message, string error = null)
        {
            return new Result(
                success: false,
                message: message,
                errors: string.IsNullOrEmpty(error) ? new List<string>() : new List<string> { error },
                statusCode: 409
            );
        }

        public static Result UnauthorizedResult(string message)
        {
            return new Result(
                success: false,
                message: message,
                errors: new List<string>(),
                statusCode: 401
            );
        }

        public static Result ForbiddenResult(string message)
        {
            return new Result(
                success: false,
                message: message,
                errors: new List<string>(),
                statusCode: 403
            );
        }

        public static Result ValidationErrorResult(string message, List<string> errors)
        {
            return new Result(
                success: false,
                message: message,
                errors: errors ?? new List<string>(),
                statusCode: 400
            );
        }
    }
}
