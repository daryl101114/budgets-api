using budget_api.Models.Entities;

namespace budget_api.Repositories.Interface
{
    public interface ITransactionRepository
    {
        Task CreateTransactionAsync(Transaction transaction);
        Task<List<Transaction>> GetTransactionsByWalletAsync(
            Guid walletId, Guid userId, int limit, DateOnly? cursorDate, DateTime? cursorCreatedAt);
        Task<Transaction?> GetTransactionByIdAsync(Guid transactionId, Guid userId);
        Task UpdateTransactionAsync(Transaction transaction);
        Task SoftDeleteAsync(Transaction transaction);
        Task<bool> SaveChangesAsync();
    }
}
