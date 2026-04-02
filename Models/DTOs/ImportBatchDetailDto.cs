namespace budget_api.Models.DTOs
{
    public class ImportBatchDetailDto : ImportBatchDto
    {
        public int PendingDuplicates { get; set; }
        public bool ReadyToConfirm => PendingDuplicates == 0;
        public List<ImportTransactionDto> Transactions { get; set; } = new();
    }
}
