using System.ComponentModel.DataAnnotations;
using budget_api.Models.Enums;

namespace budget_api.Models.DTOs
{
    public class UpdateWalletDto
    {
        [MaxLength(100)]
        public string? WalletName { get; set; }

        [MaxLength(100)]
        public string? Institution { get; set; }

        public Currency? Currency { get; set; }
    }
}
