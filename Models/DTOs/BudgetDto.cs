namespace budget_api.Models.DTOs
{
    public class BudgetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal TotalIncome { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<BudgetAllocationDto> Allocations { get; set; } = new List<BudgetAllocationDto>();
    }
}
