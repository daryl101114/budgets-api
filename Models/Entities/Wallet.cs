using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using budget_api.Models.Enums;

namespace budget_api.Models.Entities
{
    public class Wallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [MaxLength(100)]
        public string WalletName { get; set; }

        [Required]
        public WalletType WalletType { get; set; }

        [MaxLength(100)]
        public string? Institution { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Balance { get; set; } = 0.00m;

        [Required]
        public Currency Currency { get; set; }

        public DateTime? LastSynced { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}
