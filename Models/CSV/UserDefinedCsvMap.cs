using budget_api.Models.Entities;
using CsvHelper.Configuration;

namespace budget_api.Models.CSV
{
    public sealed class UserDefinedCsvMap : ClassMap<ImportCsvRecord>
    {
        public UserDefinedCsvMap(WalletImportProfile profile)
        {
            Map(m => m.Date).Name(profile.DateColumn);
            Map(m => m.Amount).Name(profile.AmountColumn);

            if (!string.IsNullOrWhiteSpace(profile.TypeColumn))
                Map(m => m.Type).Name(profile.TypeColumn);

            if (!string.IsNullOrWhiteSpace(profile.DescriptionColumn))
                Map(m => m.Description).Name(profile.DescriptionColumn).Optional();

            if (!string.IsNullOrWhiteSpace(profile.RawDescriptionColumn))
                Map(m => m.RawDescription).Name(profile.RawDescriptionColumn).Optional();

            if (!string.IsNullOrWhiteSpace(profile.BankTxnTypeColumn))
                Map(m => m.BankTxnType).Name(profile.BankTxnTypeColumn).Optional();

            if (!string.IsNullOrWhiteSpace(profile.CheckNumberColumn))
                Map(m => m.CheckNumber).Name(profile.CheckNumberColumn).Optional();

            if (!string.IsNullOrWhiteSpace(profile.BalanceAfterColumn))
                Map(m => m.BalanceAfter).Name(profile.BalanceAfterColumn).Optional();
        }
    }
}
