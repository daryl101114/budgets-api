using budget_api.Models.Entities;

namespace budget_api.Repositories.Interface
{
    public interface IImportRepository
    {
        Task CreateAsync(ImportBatch batch);
        Task<ImportBatch?> GetByIdAsync(Guid batchId, Guid userId);
        Task<IEnumerable<ImportBatch>> GetByUserIdAsync(Guid userId);
        Task<List<Transaction>> GetBatchTransactionsAsync(Guid batchId);
        Task<HashSet<string>> GetExistingHashesAsync(Guid walletId);
        Task<bool> SaveChangesAsync();
    }
}
