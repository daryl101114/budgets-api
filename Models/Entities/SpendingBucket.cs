using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.Entities
{
    public class SpendingBucket
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string BucketName { get; set; }

        public ICollection<TransactionCategory> Categories { get; set; } = new List<TransactionCategory>();
    }
}
