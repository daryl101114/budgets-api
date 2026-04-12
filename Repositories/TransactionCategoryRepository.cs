using budget_api.DbConext;
using budget_api.Models.Entities;
using budget_api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace budget_api.Repositories
{
    public class TransactionCategoryRepository : ITransactionCategoryRepository
    {
        private readonly BudgetsDbContext _context;

        public TransactionCategoryRepository(BudgetsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionCategory>> GetCategoriesAsync(Guid userId)
        {
            return await _context.TransactionCategory
                .Where(c => c.DeletedAt == null && (c.UserId == null || c.UserId == userId))
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<TransactionCategory?> GetCategoryByIdAsync(Guid categoryId)
        {
            return await _context.TransactionCategory
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.DeletedAt == null);
        }

        public async Task CreateCategoryAsync(TransactionCategory category)
        {
            await _context.TransactionCategory.AddAsync(category);
        }

        public Task UpdateCategoryAsync(TransactionCategory category)
        {
            // Entity is already tracked from GetCategoryByIdAsync; change tracker handles the update
            return Task.CompletedTask;
        }

        public Task SoftDeleteAsync(TransactionCategory category)
        {
            // Entity is already tracked from GetCategoryByIdAsync; just set the field
            category.DeletedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
