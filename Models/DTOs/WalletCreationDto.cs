using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class WalletCreationDto
    {
        public string AccountName { get; set; }
        [Required]
        public int WalletTypeId { get; set; }
        public double? Balance { get; set; } = 0.00;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Currency { get; set; }
        public string? Emoji { get; set; }
    }
}
