using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using budget_api.Models.Enums;

namespace budget_api.Models.Entities
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid WalletId { get; set; }
        [ForeignKey("WalletId")]
        public Wallet? Wallet { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public Guid? SubcategoryId { get; set; }
        [ForeignKey("SubcategoryId")]
        public TransactionCategory? Subcategory { get; set; }

        public Guid? ImportId { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(1000)]
        public string? RawDescription { get; set; }

        [MaxLength(50)]
        public string? BankTxnType { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? BalanceAfter { get; set; }

        [MaxLength(20)]
        public string? CheckNumber { get; set; }

        public bool IsRecurring { get; set; } = false;

        public Guid? RecurringRuleId { get; set; }

        [MaxLength(64)]
        public string? DuplicateHash { get; set; }

        public TransactionSource Source { get; set; } = TransactionSource.Manual;

        public bool IsDuplicate { get; set; } = false;

        public ImportStatus? ImportStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}
