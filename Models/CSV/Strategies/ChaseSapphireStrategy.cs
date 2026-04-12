using budget_api.Models.Enums;
using CsvHelper.Configuration;

namespace budget_api.Models.CSV.Strategies
{
    public class ChaseSapphireStrategy : IImportFormatStrategy
    {
        public bool CanHandle(string[] headers) =>
            headers.Any(h => h.Equals("Transaction Date", StringComparison.OrdinalIgnoreCase));

        public ClassMap<ImportCsvRecord> GetClassMap() => new ChaseSapphireCsvMap();

        public TransactionType ParseTransactionType(string type) =>
            type.Trim().ToLowerInvariant() switch
            {
                "payment" or "return" => TransactionType.Income,
                "sale"    or "fee"    => TransactionType.Expense,
                _ => throw new FormatException($"Unknown transaction type: '{type}'.")
            };

        public string CleanDescription(ImportCsvRecord record) =>
            DescriptionCleaner.CleanSapphire(record.Description ?? record.RawDescription);
    }
}
