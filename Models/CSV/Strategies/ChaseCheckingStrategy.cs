using budget_api.Models.Enums;
using CsvHelper.Configuration;

namespace budget_api.Models.CSV.Strategies
{
    public class ChaseCheckingStrategy : IImportFormatStrategy
    {
        public bool CanHandle(string[] headers) =>
            headers.Any(h => h.Equals("Posting Date", StringComparison.OrdinalIgnoreCase));

        public ClassMap<ImportCsvRecord> GetClassMap() => new ChaseCheckingCsvMap();

        public TransactionType ParseTransactionType(string type) =>
            type.Trim().ToLowerInvariant() switch
            {
                "credit" => TransactionType.Income,
                "debit"  => TransactionType.Expense,
                _ => throw new FormatException($"Unknown transaction type: '{type}'.")
            };

        public string CleanDescription(ImportCsvRecord record)
        {
            var raw = record.RawDescription ?? record.Description ?? string.Empty;
            return DescriptionCleaner.CleanChecking(raw, record.BankTxnType);
        }
    }
}
