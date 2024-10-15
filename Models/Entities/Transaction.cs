using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget_api.Models.Entities
{
    public enum TransactionType
    {
        Credit,
        Debit
    }
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        [ForeignKey("WalletId")]
        public Wallets Wallet { get; set; } = new Wallets();
        public Guid WalletId { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRecurring { get; set; }
        public TransactionType TransactionType { get; set; }
        [ForeignKey("TransactionCategoryId")]
        public TransactionCategory TransactionCategory { get; set; }
        public Guid TransactionCategoryId { get; set; }
    }
}
