using budget_api.Models.Enums;

namespace budget_api.Models.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public Guid UserId { get; set; }
        public Guid? SubcategoryId { get; set; }
        public Guid? ImportId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateOnly Date { get; set; }
        public string? Description { get; set; }
        public string? RawDescription { get; set; }
        public string? BankTxnType { get; set; }
        public decimal? BalanceAfter { get; set; }
        public string? CheckNumber { get; set; }
        public bool IsRecurring { get; set; }
        public string? DuplicateHash { get; set; }
        public TransactionSource Source { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
