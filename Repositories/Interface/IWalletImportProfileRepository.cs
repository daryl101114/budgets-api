using budget_api.Models.Entities;

namespace budget_api.Repositories.Interface
{
    public interface IWalletImportProfileRepository
    {
        Task<WalletImportProfile?> GetByWalletIdAsync(Guid walletId);
        Task AddAsync(WalletImportProfile profile);
        void Delete(WalletImportProfile profile);
        Task<bool> SaveChangesAsync();
    }
}
