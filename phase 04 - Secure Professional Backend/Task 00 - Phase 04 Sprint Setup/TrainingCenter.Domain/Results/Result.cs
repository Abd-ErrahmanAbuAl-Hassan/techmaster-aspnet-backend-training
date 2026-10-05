using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TrainingCenter.Domain.Results
{
    public class Result<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public T Data { get; set; }
        public List<string> Errors { get; }

        private Result(bool success, string message, T data, List<string> errors)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public static Result<T> SuccessResult(T data, string message = "Operation completed successfully")
        {
            return new Result<T>
            (
                success: true,
                message: message,
                data: data,
                errors: new List<string>()
            );
        }

        public static Result<T> FailureResult(string message, List<string> errors = null)
        {
            return new Result<T>
            (
                success: false,
                message: message,
                data: default!,
                errors: errors ?? new List<string>()
            );
        }

        public static Result<T> FailureResult(string message, string error)
        {
            return new Result<T>
            (
               success: false,
               message: message,
               data: default!,
               errors: new List<string> { error }
            );
        }
    }
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        private Result(bool success, string message, List<string> errors)
        {
            Success = success;
            Message = message;
            Errors = errors;
        }

        public static Result SuccessResult(string message = "Operation completed successfully")
        {
            return new Result
            (
                success: false,
                message: message,
                errors: new List<string>()
            );
        }

        public static Result FailureResult(string message, List<string> errors = null)
        {
            return new Result
            (
                success: false,
                message: message,
                errors: errors ?? new List<string>()
            );
        }

        public static Result FailureResult(string message, string error)
        {
            return new Result
            (
                success: false,
                message: message,
                errors: new List<string>() { error }
            );
        }
    }
}
