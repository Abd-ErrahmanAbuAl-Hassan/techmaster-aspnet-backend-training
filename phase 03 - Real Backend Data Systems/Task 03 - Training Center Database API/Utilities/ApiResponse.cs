namespace Task_03___Training_Center_Database_API.Utilities
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}