using budget_api.Models.Enums;
using CsvHelper.Configuration;

namespace budget_api.Models.CSV.Strategies
{
    public interface IImportFormatStrategy
    {
        bool CanHandle(string[] headers);
        ClassMap<ImportCsvRecord> GetClassMap();
        TransactionType ParseTransactionType(string type);
        string CleanDescription(ImportCsvRecord record) => record.Description ?? string.Empty;
    }
}
