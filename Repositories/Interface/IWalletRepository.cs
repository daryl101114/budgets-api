using budget_api.Models.Entities;

namespace budget_api.Repositories.Interface
{
    public interface IWalletRepository
    {
        Task CreateWalletAsync(Wallet wallet);
        Task<ICollection<Wallet>> GetWalletsByUserIdAsync(Guid userId);
        Task<Wallet?> GetWalletByIdAsync(Guid walletId, Guid userId);
        Task UpdateWalletAsync(Wallet wallet);
        Task SoftDeleteWalletAsync(Wallet wallet);
        Task<bool> SaveChangesAsync();
    }
}
