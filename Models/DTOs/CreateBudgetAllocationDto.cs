using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class CreateBudgetAllocationDto
    {
        [Required]
        public Guid BucketId { get; set; }

        [Required]
        [Range(0.01, 100.00, ErrorMessage = "Percentage must be between 0.01 and 100.")]
        public decimal Percentage { get; set; }
    }
}
