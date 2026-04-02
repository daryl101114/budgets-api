using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget_api.Models.Entities
{
    public class TransactionCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid BucketId { get; set; }
        public SpendingBucket? Bucket { get; set; }

        public Guid? UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        public bool IsFixed { get; set; } = false;

        public string? Icon { get; set; }

        public string? Color { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }
    }
}
