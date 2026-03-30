namespace budget_api.Models.DTOs
{
    public class TransactionUpdateDto
    {
        public Guid Id { get; set; }
        public string TransactionName { get; set; }
        public string? TransactionDescription { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool? IsRecurring { get; set; }
        public string TransactionType { get; set; }
        public Guid TransactionCategoryId { get; set; }
    }
}
