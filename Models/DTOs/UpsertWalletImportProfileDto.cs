namespace budget_api.Models.DTOs
{
    public class UpsertWalletImportProfileDto
    {
        public string DateColumn { get; set; } = string.Empty;
        public string AmountColumn { get; set; } = string.Empty;
        public string? TypeColumn { get; set; }
        public string? DescriptionColumn { get; set; }
        public string? RawDescriptionColumn { get; set; }
        public string? BankTxnTypeColumn { get; set; }
        public string? CheckNumberColumn { get; set; }
        public string? BalanceAfterColumn { get; set; }
        public string? IncomeTypeValues { get; set; }
        public string? ExpenseTypeValues { get; set; }
    }
}
