using budget_api.Models.Entities;

namespace budget_api.Repositories.Interface
{
    public interface ITransactionRepository
    {
        Task CreateTransactionAsync(Transaction transaction);
        Task<IEnumerable<TransactionCategory>> GetTransactionCategoriesAsync();
        Task<bool> SaveTransactionAsync();
        Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid walletId);
    }
}
