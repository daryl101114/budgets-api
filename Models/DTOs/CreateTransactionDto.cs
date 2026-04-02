using System.ComponentModel.DataAnnotations;
using budget_api.Models.Enums;

namespace budget_api.Models.DTOs
{
    public class CreateTransactionDto
    {
        [Required]
        public Guid WalletId { get; set; }

        public Guid? SubcategoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsRecurring { get; set; } = false;
    }
}
