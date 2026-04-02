using budget_api.DbConext;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace budget_api.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly BudgetsDbContext _context;

        public BudgetRepository(BudgetsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Budget>> GetBudgetsByUserIdAsync(Guid userId)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId && b.DeletedAt == null)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<Budget?> GetBudgetByIdAsync(Guid budgetId, Guid userId)
        {
            return await _context.Budgets
                .Include(b => b.Allocations)
                    .ThenInclude(a => a.Bucket)
                .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId && b.DeletedAt == null);
        }

        public async Task<Budget?> GetActiveBudgetAsync(Guid userId)
        {
            return await _context.Budgets
                .Include(b => b.Allocations)
                    .ThenInclude(a => a.Bucket)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.IsActive && b.DeletedAt == null);
        }

        public async Task CreateBudgetAsync(Budget budget)
        {
            await _context.Budgets.AddAsync(budget);
        }

        public async Task DeactivateAllBudgetsAsync(Guid userId)
        {
            await _context.Budgets
                .Where(b => b.UserId == userId && b.IsActive && b.DeletedAt == null)
                .ExecuteUpdateAsync(b => b.SetProperty(x => x.IsActive, false));
        }

        public Task SoftDeleteAsync(Budget budget)
        {
            budget.DeletedAt = DateTime.UtcNow;
            budget.IsActive = false;
            return Task.CompletedTask;
        }

        public async Task<BudgetAllocation?> GetAllocationByIdAsync(Guid allocationId, Guid budgetId)
        {
            return await _context.BudgetAllocations
                .FirstOrDefaultAsync(a => a.Id == allocationId && a.BudgetId == budgetId);
        }

        public async Task AddAllocationAsync(BudgetAllocation allocation)
        {
            await _context.BudgetAllocations.AddAsync(allocation);
        }

        public Task RemoveAllocationAsync(BudgetAllocation allocation)
        {
            _context.BudgetAllocations.Remove(allocation);
            return Task.CompletedTask;
        }

        public async Task<decimal> GetTotalPercentageAsync(Guid budgetId, Guid? excludeAllocationId = null)
        {
            var query = _context.BudgetAllocations.Where(a => a.BudgetId == budgetId);
            if (excludeAllocationId.HasValue)
                query = query.Where(a => a.Id != excludeAllocationId.Value);
            return await query.SumAsync(a => a.Percentage);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
