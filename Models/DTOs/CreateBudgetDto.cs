using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class CreateBudgetDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "TotalIncome must be positive.")]
        public decimal TotalIncome { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public bool IsActive { get; set; } = false;
    }
}
