namespace budget_api.Models.Entities
{
    public class WalletImportProfile
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }

        // Required column mappings
        public string DateColumn { get; set; } = string.Empty;
        public string AmountColumn { get; set; } = string.Empty;

        // Optional column mappings
        public string? TypeColumn { get; set; }
        public string? DescriptionColumn { get; set; }
        public string? RawDescriptionColumn { get; set; }
        public string? BankTxnTypeColumn { get; set; }
        public string? CheckNumberColumn { get; set; }
        public string? BalanceAfterColumn { get; set; }

        // Comma-separated type values e.g. "CREDIT,Payment,Return"
        public string? IncomeTypeValues { get; set; }
        public string? ExpenseTypeValues { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Wallet Wallet { get; set; } = null!;
    }
}
