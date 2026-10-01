namespace Task_06___API_Standards___Refactor_Pack.DTOs
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ErrorCode { get; set; } = 200;
        public List<string> Errors { get; set; } = new();
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}
