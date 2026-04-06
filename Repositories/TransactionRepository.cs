using budget_api.DbConext;
using budget_api.Models.Entities;
using budget_api.Models.Enums;
using budget_api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace budget_api.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BudgetsDbContext _context;

        public TransactionRepository(BudgetsDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<List<Transaction>> GetTransactionsByWalletAsync(
            Guid walletId, Guid userId, int limit, DateOnly? cursorDate, DateTime? cursorCreatedAt)
        {
            var query = _context.Transactions
                .Include(t => t.Subcategory)
                .Where(t => t.WalletId == walletId && t.UserId == userId && t.DeletedAt == null
                         && (t.Source == TransactionSource.Manual || t.ImportStatus == ImportStatus.Confirmed));

            if (cursorDate.HasValue && cursorCreatedAt.HasValue)
            {
                query = query.Where(t =>
                    t.Date < cursorDate.Value ||
                    (t.Date == cursorDate.Value && t.CreatedAt < cursorCreatedAt.Value));
            }

            return await query
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId, Guid userId)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId && t.DeletedAt == null);
        }

        public Task UpdateTransactionAsync(Transaction transaction)
        {
            transaction.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task SoftDeleteAsync(Transaction transaction)
        {
            transaction.DeletedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
