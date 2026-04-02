using System.ComponentModel.DataAnnotations;
using budget_api.Models.Enums;

namespace budget_api.Models.DTOs
{
    public class CreateWalletDto
    {
        [Required]
        [MaxLength(100)]
        public string WalletName { get; set; }

        [Required]
        public WalletType WalletType { get; set; }

        [MaxLength(100)]
        public string? Institution { get; set; }

        public decimal Balance { get; set; } = 0.00m;

        [Required]
        public Currency Currency { get; set; }
    }
}
