using System.ComponentModel.DataAnnotations;
using budget_api.Models.Enums;

namespace budget_api.Models.DTOs
{
    public class UpdateTransactionDto
    {
        [MaxLength(500)]
        public string? Description { get; set; }

        public DateOnly? Date { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal? Amount { get; set; }

        public TransactionType? TransactionType { get; set; }

        public Guid? SubcategoryId { get; set; }

        public bool? IsRecurring { get; set; }
    }
}
