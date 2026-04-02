using budget_api.Models.Enums;

namespace budget_api.Models.DTOs
{
    public class WalletDto
    {
        public Guid Id { get; set; }
        public string WalletName { get; set; }
        public WalletType WalletType { get; set; }
        public string? Institution { get; set; }
        public decimal Balance { get; set; }
        public Currency Currency { get; set; }
        public DateTime? LastSynced { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
