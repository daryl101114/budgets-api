using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class CreateTransactionCategoryDto
    {
        [Required]
        public Guid BucketId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        public bool IsFixed { get; set; } = false;

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }
    }
}
