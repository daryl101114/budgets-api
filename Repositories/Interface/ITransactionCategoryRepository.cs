using budget_api.Models.Entities;

namespace budget_api.Repositories.Interface
{
    public interface ITransactionCategoryRepository
    {
        Task<IEnumerable<TransactionCategory>> GetCategoriesAsync(Guid userId);
        Task<TransactionCategory?> GetCategoryByIdAsync(Guid categoryId);
        Task CreateCategoryAsync(TransactionCategory category);
        Task UpdateCategoryAsync(TransactionCategory category);
        Task SoftDeleteAsync(TransactionCategory category);
        Task<bool> SaveChangesAsync();
    }
}
