using budget_api.Models.Enums;
using CsvHelper.Configuration;

namespace budget_api.Models.CSV.Strategies
{
    public class GenericStrategy : IImportFormatStrategy
    {
        public bool CanHandle(string[] headers) =>
            headers.Any(h => h.Equals("Date", StringComparison.OrdinalIgnoreCase));

        public ClassMap<ImportCsvRecord> GetClassMap() => new GenericCsvMap();

        public TransactionType ParseTransactionType(string type) =>
            type.Trim().ToLowerInvariant() switch
            {
                "income" or "credit" => TransactionType.Income,
                "expense" or "debit" => TransactionType.Expense,
                _ => throw new FormatException($"Unknown transaction type: '{type}'.")
            };
    }
}
