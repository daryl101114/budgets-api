using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class UpdateTransactionCategoryDto
    {
        [MaxLength(100)]
        public string? CategoryName { get; set; }

        public Guid? BucketId { get; set; }

        public bool? IsFixed { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }
    }
}
