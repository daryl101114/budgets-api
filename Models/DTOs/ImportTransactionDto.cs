namespace budget_api.Models.DTOs
{
    public class ImportTransactionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string? Description { get; set; }
        public string? RawDescription { get; set; }
        public string? BankTxnType { get; set; }
        public bool IsDuplicate { get; set; }
        public string ImportStatus { get; set; } = string.Empty;
    }
}
