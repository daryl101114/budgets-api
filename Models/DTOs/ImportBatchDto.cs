namespace budget_api.Models.DTOs
{
    public class ImportBatchDto
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int ImportedCount { get; set; }
        public int DuplicateCount { get; set; }
        public int SkippedCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? RolledBackAt { get; set; }
    }
}
