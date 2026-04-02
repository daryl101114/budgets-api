using budget_api.Models.Entities;

namespace budget_api.Repositories.Interface
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<Budget>> GetBudgetsByUserIdAsync(Guid userId);
        Task<Budget?> GetBudgetByIdAsync(Guid budgetId, Guid userId);
        Task<Budget?> GetActiveBudgetAsync(Guid userId);
        Task CreateBudgetAsync(Budget budget);
        Task DeactivateAllBudgetsAsync(Guid userId);
        Task SoftDeleteAsync(Budget budget);
        Task<BudgetAllocation?> GetAllocationByIdAsync(Guid allocationId, Guid budgetId);
        Task AddAllocationAsync(BudgetAllocation allocation);
        Task RemoveAllocationAsync(BudgetAllocation allocation);
        Task<decimal> GetTotalPercentageAsync(Guid budgetId, Guid? excludeAllocationId = null);
        Task<bool> SaveChangesAsync();
    }
}
