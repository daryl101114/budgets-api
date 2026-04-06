using CsvHelper.Configuration;

namespace budget_api.Models.CSV
{
    // Headers: Details, Posting Date, Description, Amount, Type, Balance, Check or Slip #
    //
    // Details  → Type        (CREDIT/DEBIT  → TransactionType)
    // Type     → BankTxnType (ACH_DEBIT, DEBIT_CARD, ATM, etc.)
    public sealed class ChaseCheckingCsvMap : ClassMap<ImportCsvRecord>
    {
        public ChaseCheckingCsvMap()
        {
            Map(m => m.Date).Name("Posting Date");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.Type).Name("Details");
            Map(m => m.Description).Name("Description").Optional();
            Map(m => m.RawDescription).Name("Description").Optional();
            Map(m => m.BankTxnType).Name("Type").Optional();
            Map(m => m.CheckNumber).Name("Check or Slip #").Optional();
            Map(m => m.BalanceAfter).Name("Balance").Optional();
        }
    }
}
