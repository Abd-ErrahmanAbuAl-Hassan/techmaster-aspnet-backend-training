namespace Task_05_Business_Rules_Data_Integrity.Utilities
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