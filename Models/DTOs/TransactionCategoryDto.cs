namespace budget_api.Models.DTOs
{
    public class TransactionCategoryDto
    {
        public Guid Id { get; set; }
        public Guid BucketId { get; set; }
        public Guid? UserId { get; set; }
        public string CategoryName { get; set; }
        public bool IsFixed { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
