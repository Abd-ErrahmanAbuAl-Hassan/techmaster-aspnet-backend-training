namespace BookStoreApi.Utilities
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        private Result()
        {
            Errors = new List<string>();
        }

        public static Result<T> SuccessResult(T data, string message = "Operation completed successfully")
        {
            return new Result<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static Result<T> FailureResult(string message, List<string> errors = null)
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>()
            };
        }

        public static Result<T> FailureResult(string message, string error)
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = new List<string> { error }
            };
        }
    }
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        private Result()
        {
            Errors = new List<string>();
        }

        public static Result SuccessResult(string message = "Operation completed successfully")
        {
            return new Result
            {
                Success = true,
                Message = message,
                Errors = new List<string>()
            };
        }

        public static Result FailureResult(string message, List<string> errors = null)
        {
            return new Result
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static Result FailureResult(string message, string error)
        {
            return new Result
            {
                Success = false,
                Message = message,
                Errors = new List<string> { error }
            };
        }
    }
}