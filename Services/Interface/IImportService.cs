using budget_api.Models.DTOs;

namespace budget_api.Services.Interface
{
    public interface IImportService
    {
        Task<ImportBatchDto> UploadAsync(Guid walletId, Guid userId, IFormFile file);
        Task<IEnumerable<ImportBatchDto>> GetBatchesAsync(Guid userId);
        Task<ImportBatchDetailDto?> GetBatchDetailAsync(Guid batchId, Guid userId);
        Task BulkApproveAsync(Guid batchId, Guid userId, List<Guid> transactionIds);
        Task BulkRejectAsync(Guid batchId, Guid userId, List<Guid> transactionIds);
        Task ConfirmAsync(Guid batchId, Guid userId);
        Task RollbackAsync(Guid batchId, Guid userId);
    }
}
