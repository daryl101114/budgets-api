using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class UpdateBudgetDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "TotalIncome must be positive.")]
        public decimal? TotalIncome { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }
    }
}
