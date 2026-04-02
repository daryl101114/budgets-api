using System.ComponentModel.DataAnnotations;
using budget_api.Models.Enums;

namespace budget_api.Models.Entities
{
    public class ImportBatch
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid WalletId { get; set; }
        public Wallet? Wallet { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public ImportBatchStatus Status { get; set; } = ImportBatchStatus.Reviewing;

        public int TotalRows { get; set; }
        public int ImportedCount { get; set; }
        public int DuplicateCount { get; set; }
        public int SkippedCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? RolledBackAt { get; set; }
    }
}
