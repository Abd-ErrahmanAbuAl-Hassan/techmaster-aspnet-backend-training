using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TrainingCenter.Domain.Results
{
    public class Result<T>
    {
        public bool Success { get;}
        public string Message { get; }
        public T Data { get; }
        public List<string> Errors { get; }
        public int StatusCode { get; } 
        private Result(bool success, string message, T data, List<string> errors , int statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
            StatusCode = statusCode;
        }

        public static Result<T> SuccessResult(T data, string message = "Operation completed successfully", int statusCode = 200)
        {
            return new Result<T>
            (
                success: true,
                message: message,
                data: data,
                errors: new List<string>(),
                statusCode: statusCode
            );
        }
       
        public static Result<T> FailureResult(string message, List<string> errors = null, int statusCode = 500)
        {
            return new Result<T>
            (
                success: false,
                message: message,
                data: default!,
                errors: errors ?? new List<string>(),
                statusCode: statusCode
            );
        }

        public static Result<T> FailureResult(string message, string error, int statusCode = 500)
        {
            return new Result<T>
            (
               success: false,
               message: message,
               data: default!,
               errors: new List<string> { error },
               statusCode: statusCode
            );
        }
    }
    public class Result
    {
        public bool Success { get;  }
        public string Message { get;  }
        public List<string> Errors { get; }
        public int StatusCode { get; }

        private Result(bool success, string message, List<string> errors, int statusCode)
        {
            Success = success;
            Message = message;
            Errors = errors;
            StatusCode = statusCode;
        }

        public static Result SuccessResult(string message = "Operation completed successfully",int statusCode = 200)
        {
            return new Result
            (
                success: false,
                message: message,
                errors: new List<string>(),
                statusCode: statusCode
            );
        }

        public static Result FailureResult(string message, List<string> errors = null, int statusCode = 500)
        {
            return new Result
            (
                success: false,
                message: message,
                errors: errors ?? new List<string>(),
                statusCode: statusCode
            );
        }

        public static Result FailureResult(string message, string error, int statusCode = 500)
        {
            return new Result
            (
                success: false,
                message: message,
                errors: new List<string>() { error },
                statusCode:statusCode
            );
        }
    }
}
