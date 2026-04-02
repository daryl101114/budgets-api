using budget_api.Models.Entities;

namespace budget_api.Repositories.Interface
{
    public interface ITransactionRepository
    {
        Task CreateTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetTransactionsByWalletAsync(Guid walletId, Guid userId);
        Task<Transaction?> GetTransactionByIdAsync(Guid transactionId, Guid userId);
        Task UpdateTransactionAsync(Transaction transaction);
        Task SoftDeleteAsync(Transaction transaction);
        Task<bool> SaveChangesAsync();
    }
}
