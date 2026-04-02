namespace budget_api.Models.CSV
{
    public class ImportCsvRecord
    {
        public string Date { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? RawDescription { get; set; }
        public string? BankTxnType { get; set; }
        public string? CheckNumber { get; set; }
        public decimal? BalanceAfter { get; set; }
    }
}
