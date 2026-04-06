using budget_api.Models.Entities;
using budget_api.Models.Enums;
using CsvHelper.Configuration;

namespace budget_api.Models.CSV.Strategies
{
    public class UserDefinedStrategy : IImportFormatStrategy
    {
        private readonly WalletImportProfile _profile;

        public UserDefinedStrategy(WalletImportProfile profile)
        {
            _profile = profile;
        }

        // Always selected — only used when a wallet has a saved profile
        public bool CanHandle(string[] headers) => true;

        public ClassMap<ImportCsvRecord> GetClassMap() => new UserDefinedCsvMap(_profile);

        public TransactionType ParseTransactionType(string type)
        {
            // If no type column configured, fall back to sign-based (handled upstream)
            if (string.IsNullOrWhiteSpace(_profile.TypeColumn))
                throw new InvalidOperationException("No type column configured.");

            var income = _profile.IncomeTypeValues?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? [];

            var expense = _profile.ExpenseTypeValues?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? [];

            if (income.Any(v => v.Equals(type, StringComparison.OrdinalIgnoreCase)))
                return TransactionType.Income;

            if (expense.Any(v => v.Equals(type, StringComparison.OrdinalIgnoreCase)))
                return TransactionType.Expense;

            throw new FormatException($"Unknown transaction type: '{type}'. Expected one of: {string.Join(", ", income.Concat(expense))}");
        }
    }
}
