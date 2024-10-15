using budget_api.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace budget_api.Models.DTOs
{
    public class WalletsDto
    {
        public Guid Id { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public double? Balance { get; set; } = 0.00;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public WalletTypeDto? WalletType { get; set; }
        public string? Currency { get; set; }
        public string? Emoji { get; set; }
    }
}
