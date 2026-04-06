using budget_api.Models.DTOs;
using budget_api.Models.Enums;

namespace budget_api.Services.Interface
{
    public interface IImportService
    {
        Task<ImportBatchDto> UploadAsync(Guid walletId, Guid userId, IFormFile file);
        Task<IEnumerable<ImportBatchDto>> GetBatchesAsync(Guid userId, ImportBatchStatus? status = null);
        Task<ImportBatchDetailDto?> GetBatchDetailAsync(Guid batchId, Guid userId);
        Task BulkApproveAsync(Guid batchId, Guid userId, List<Guid> transactionIds);
        Task BulkRejectAsync(Guid batchId, Guid userId, List<Guid> transactionIds);
        Task ConfirmAsync(Guid batchId, Guid userId);
        Task RollbackAsync(Guid batchId, Guid userId);
    }
}
