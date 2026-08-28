namespace Task_02___Student_Management_API.Utilities
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public T? Data { get; set; }
        public int ErrorCode { get; set; }
    }
}
