using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget_api.Models.Entities
{
    public class TransactionCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string TransactionCategoryName { get; set; }
        public bool IsFixed { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }
}
