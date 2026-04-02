namespace budget_api.Models.DTOs
{
    public class BudgetAllocationDto
    {
        public Guid Id { get; set; }
        public Guid BucketId { get; set; }
        public string BucketName { get; set; }
        public decimal Percentage { get; set; }
        public decimal AllocatedAmount { get; set; }
    }
}
