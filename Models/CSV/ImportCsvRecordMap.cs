using CsvHelper.Configuration;

namespace budget_api.Models.CSV
{
    public sealed class GenericCsvMap : ClassMap<ImportCsvRecord>
    {
        public GenericCsvMap()
        {
            Map(m => m.Date).Name("Date");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.Type).Name("Type");
            Map(m => m.Description).Name("Description").Optional();
            Map(m => m.RawDescription).Name("RawDescription").Optional();
            Map(m => m.BankTxnType).Name("BankTxnType").Optional();
            Map(m => m.CheckNumber).Name("CheckNumber").Optional();
            Map(m => m.BalanceAfter).Name("BalanceAfter").Optional();
        }
    }
}
