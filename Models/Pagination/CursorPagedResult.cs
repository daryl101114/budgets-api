namespace budget_api.Models.Pagination
{
    public class CursorPagedResult<T>
    {
        public List<T> Data { get; set; } = [];
        public string? NextCursor { get; set; }
        public bool HasMore { get; set; }
    }
}
