using budget_api.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class TransactionsDto
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public Wallets? Wallet { get; set; }
        public string TransactionName { get; set; }
        public string? TransactionDescription { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRecurring { get; set; }
        public string TransactionType { get; set; }
        public TransactionCategory? TransactionCategory { get; set; }
        public Guid TransactionCategoryId { get; set; }
    }
}
