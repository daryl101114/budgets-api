using budget_api.Models.DTOs;

namespace budget_api.Services.Interface
{
    public interface IBudgetService
    {
        Task<IEnumerable<BudgetDto>> GetBudgetsAsync(Guid userId);
        Task<BudgetDto?> GetBudgetByIdAsync(Guid budgetId, Guid userId);
        Task<BudgetDto?> GetActiveBudgetAsync(Guid userId);
        Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto dto, Guid userId);
        Task UpdateBudgetAsync(Guid budgetId, Guid userId, UpdateBudgetDto dto);
        Task ActivateBudgetAsync(Guid budgetId, Guid userId);
        Task DeleteBudgetAsync(Guid budgetId, Guid userId);
        Task AddAllocationAsync(Guid budgetId, Guid userId, CreateBudgetAllocationDto dto);
        Task UpdateAllocationAsync(Guid budgetId, Guid allocationId, Guid userId, UpdateBudgetAllocationDto dto);
        Task RemoveAllocationAsync(Guid budgetId, Guid allocationId, Guid userId);
    }
}
