namespace budget_api.Models.Entities
{
    public class WalletTypes
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public ICollection<Wallets> Wallets { get; set; } = new List<Wallets>();
    }
}
