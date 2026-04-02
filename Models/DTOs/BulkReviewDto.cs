namespace budget_api.Models.DTOs
{
    public class BulkReviewDto
    {
        public List<Guid> TransactionIds { get; set; } = new();
    }
}
