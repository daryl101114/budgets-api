using CsvHelper.Configuration;

namespace budget_api.Models.CSV
{
    // Headers: Transaction Date, Post Date, Description, Category, Type, Amount, Memo
    //
    // Type  → Type        (Sale/Payment/Return → TransactionType)
    // Memo  → RawDescription (falls back to Description in service if empty)
    public sealed class ChaseSapphireCsvMap : ClassMap<ImportCsvRecord>
    {
        public ChaseSapphireCsvMap()
        {
            Map(m => m.Date).Name("Transaction Date");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.Type).Name("Type");
            Map(m => m.Description).Name("Description").Optional();
            Map(m => m.RawDescription).Name("Memo").Optional();
        }
    }
}
