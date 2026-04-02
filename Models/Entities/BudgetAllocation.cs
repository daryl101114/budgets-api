using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget_api.Models.Entities
{
    public class BudgetAllocation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid BudgetId { get; set; }
        public Budget? Budget { get; set; }

        [Required]
        public Guid BucketId { get; set; }
        public SpendingBucket? Bucket { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Percentage { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal AllocatedAmount { get; set; }
    }
}
